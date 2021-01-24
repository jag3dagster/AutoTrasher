using System.Linq;
using AutoTrasher.Helpers;
using Microsoft.Xna.Framework.Input;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace AutoTrasher
{
	/// <summary>
	/// The mod entry point.
	/// </summary>
	public class ModEntry : Mod
	{
		internal static ModConfig Config; 

		/// <summary>
		/// The mod entry point, called after the mod is first loaded.
		/// </summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			Config = helper.ReadConfig<ModConfig>();

			//helper.Events.Player.InventoryChanged += this.OnInventoryChanged;
			helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			helper.Events.Input.ButtonPressed += OnButtonPressed;
		}

		private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (Game1.hudMessages.Count == 0) return;

			var localMessages = Game1.hudMessages.ToList();

			foreach (var message in localMessages)
			{
				if (message is CustomHUDMessage) continue;

				var item = Helper.Reflection.GetField<Item>(message, "messageSubject")?.GetValue() as Object;
				var subjectName = item?.Name;

				if (item == null || !Config.TrashItems.Contains(subjectName)) continue;

				Game1.hudMessages.Remove(message);
				var cMessage = new CustomHUDMessage(null, item.Stack, message.add, message.color, item);

				var player = Game1.player;
				var itemStack = item.Stack == 1 ? "" : $" x{item.Stack}";

				if (Config.UseShippingBin)
				{
					cMessage.CMessage = $"Auto-sent {item.DisplayName}{itemStack} to the shipping bin";
					MoveItemToShippingBin(player, item);
				}
				else
				{
					var reclamationPrice = RemoveItemFromInventory(player, item);
					cMessage.CMessage = $"Auto-trashed {item.DisplayName}{itemStack}" +
						(reclamationPrice > 0 ? $" and received {reclamationPrice} coin(s)" : "");
				}

				Game1.hudMessages.Add(cMessage);
			}
		}

		/// <summary>
		/// Raised after items are added or removed to a player's inventory.
		/// NOTE: This event is currently only raised for the current player.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
		{
			var logMessage = "No change";

			if (!Config.Active) return;
			if (!e.IsLocalPlayer) return;

			var curPlayer = e.Player;

			if (e.Added.Any())
			{
				foreach (var i in e.Added)
				{
					logMessage = $"{curPlayer.Name}: Received [{i.Name}] x{i.Stack}";

					if (!Config.TrashItems.Contains(i.Name)) continue;

					if (Config.UseShippingBin)
					{
						MoveItemToShippingBin(curPlayer, i);
					}
					else
					{
						RemoveItemFromInventory(curPlayer, i);
					}
				}
			}
			else if (e.Removed.Any())
			{
				foreach (var i in e.Removed)
				{
					logMessage = $"{curPlayer.Name}: Lost [{i.Name}] x{i.Stack}";
				}
			}
			else if (e.QuantityChanged.Any())
			{
				foreach (var i in e.QuantityChanged)
				{
					logMessage = $"{curPlayer.Name}: [{i.Item.Name}] changed from [{i.OldSize}] to [{i.NewSize}]";
				}
			}

			this.Monitor.Log(logMessage);
		}

		/// <summary>
		/// Raised after the player presses a button on their keyboard, controller, or mouse.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			if (Game1.GetKeyboardState().IsKeyDown(Keys.LeftAlt) ||
			    Game1.GetKeyboardState().IsKeyDown(Keys.RightAlt))
			{
				if (Game1.activeClickableMenu == null)
				{
					if (e.Button == Config.ToggleTrasher)
					{
						ToggleTrasher();
					}
					else if (e.Button == Config.ToggleShipping)
					{
						ToggleShipping();
					}
				}
				else
				{
					if (e.Button == Config.AddTrash)
					{
						AddTrash();
					}
				}
			}
		}

		/// <summary>
		/// Helper method to toggle the AutoTrasher on and off
		/// </summary>
		private void ToggleTrasher()
		{
			Config.Active = !Config.Active;
			Helper.WriteConfig(Config);

			var message = "AutoTrasher has been " + (Config.Active ? "ACTIVATED" : "DEACTIVATED");

			Monitor.Log(message);
			Game1.addHUDMessage(new HUDMessage(message, null));

			if (Config.Active)
			{
				var trashItemsInInventory = Game1.player.Items
					.Where(item => item != null && Config.TrashItems.Contains(item.Name))
					.ToList();

				if (trashItemsInInventory.Any())
				{
					foreach (var item in trashItemsInInventory)
					{
						if (Config.UseShippingBin)
						{
							MoveItemToShippingBin(Game1.player, item);
						}
						else
						{
							RemoveItemFromInventory(Game1.player, item);
						}
					}
				}
			}
		}

		/// <summary>
		/// Helper method to toggle the AutoTrasher to send to the Shipping Bin or the player's Trash Can
		/// </summary>
		private void ToggleShipping()
		{
			Config.UseShippingBin = !Config.UseShippingBin;
			Helper.WriteConfig(Config);

			var message = "AutoTrasher will now send items to " + (Config.UseShippingBin ? "the Shipping Bin" : "your Trash Can");

			Monitor.Log(message);
			Game1.addHUDMessage(new HUDMessage(message, null));
		}

		/// <summary>
		/// Helper method to add the <see cref="Object"/> that is currently being hovered over by the user to their Trash List
		/// </summary>
		private void AddTrash()
		{
			if ((Game1.activeClickableMenu as GameMenu)?.GetCurrentPage() is InventoryPage)
			{
				var inventoryMenu = (((GameMenu)Game1.activeClickableMenu).GetCurrentPage() as InventoryPage)?.inventory;
				var hoveredItem = inventoryMenu?.getItemAt(Game1.getMouseX(), Game1.getMouseY());
				var notifMessage = "";

				if (hoveredItem != null)
				{
					if (!(hoveredItem is Object)) // Item is not a general Object
					{
						notifMessage = $"Cannot add {hoveredItem.DisplayName.ToUpper()} to the Trash List";
					}
					else if (!Config.TrashItems.Contains(hoveredItem.Name)) // Item IS NOT already in the Trash List
					{
						Config.TrashItems.Add(hoveredItem.Name);
						Config.TrashItems.Sort();
						Helper.WriteConfig(Config);

						notifMessage = $"{hoveredItem.DisplayName} has been added to the Trash List";
					}
					else // Item IS already in the Trash List
					{
						notifMessage = $"{hoveredItem.DisplayName} is already on the Trash List";
					}
				}
				else // No item selected
				{
					Monitor.Log("No item selected when attempting to add item to Trash List");
				}

				if (notifMessage != "")
				{
					Monitor.Log(notifMessage);
					Game1.addHUDMessage(new HUDMessage(notifMessage, null));
				}
			}
		}

		/// <summary>
		/// Helper method to move an <paramref name="item"/> to the Shipping Bin from the <paramref name="player"/>'s inventory
		/// </summary>
		/// <param name="player"><see cref="Farmer"/> to get the shipping bin inventory of</param>
		/// <param name="item"><see cref="Item"/> to be moved to the shipping bin from the <paramref name="player"/>'s inventory</param>
		private void MoveItemToShippingBin(Farmer player, Item item)
		{
			Game1.getFarm().getShippingBin(player).Add(item);
			player.removeItemFromInventory(item);
			Game1.getFarm().lastItemShipped = item;
			Game1.soundBank.PlayCue("Ship");

			var notifMessage = $"Auto-sent {item.DisplayName} x{item.Stack} to the shipping bin";

			//Monitor.Log(notifMessage);
			//Game1.addHUDMessage(new HUDMessage(notifMessage, null));
		}

		/// <summary>
		/// Helper method to move an <paramref name="item"/> to the Trash Can from the <paramref name="player"/>'s inventory
		/// </summary>
		/// <param name="player"><see cref="Farmer"/> to be removing the <paramref name="item"/> from</param>
		/// <param name="item"><see cref="Item"/> to be moved to the shipping bin from the <paramref name="player"/>'s inventory</param>
		private int RemoveItemFromInventory(Farmer player, Item item)
		{
			var reclamationPrice = Utility.getTrashReclamationPrice(item, player);

			if (reclamationPrice > 0)
			{
				player.Money += reclamationPrice;
				Game1.soundBank.PlayCue("coin");
			}

			player.removeItemFromInventory(item);
			Game1.soundBank.PlayCue("trashcan");

			return reclamationPrice;

			//var notifMessage = $"Auto-trashed {item.DisplayName} x{item.Stack}" +
			//	(reclamationPrice > 0 ? $" and received {reclamationPrice} coin(s)" : "");

			//Monitor.Log(notifMessage);
			//Game1.addHUDMessage(new HUDMessage(notifMessage, null));
		}
	}
}
