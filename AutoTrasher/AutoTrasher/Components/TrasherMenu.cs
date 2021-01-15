using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace AutoTrasher.Components
{
	internal enum Tab
	{
		TrashList,
		Options
	}

	internal class TrasherMenu : IClickableMenu
	{
		private const string TrashListTabTitle = "Trash List";
		private const string OptionsTabTitle = "Options";
		private const int ItemsPerPage = 10;

		private readonly ModConfig Config;
		private readonly IMonitor Monitor;
		private readonly IManifest Manifest;

		private ClickableComponent Title;
		private List<ClickableComponent> Tabs = new List<ClickableComponent>();
		private ClickableTextureComponent UpArrow;
		private ClickableTextureComponent DownArrow;
		private ClickableTextureComponent ScrollBar;

		private readonly List<ClickableComponent> OptionSlots = new List<ClickableComponent>();
		private readonly List<OptionsElement> Options = new List<OptionsElement>();

		private int CurrentItemIndex;
		private Rectangle ScrollBarRunner;

		private readonly Tab CurrentTab;

		public TrasherMenu(ModConfig config, IMonitor monitor, IManifest manifest)
		{
			this.Config = config;
			this.Monitor = monitor;
			this.Manifest = manifest;
			this.CurrentTab = Tab.TrashList;
			this.ResetComponents();
		}

		private void SetOptions()
		{
			this.Options.Clear();

			switch (this.CurrentTab)
			{
				case Tab.TrashList:
					this.GenerateTrashListMenu();
					break;
				case Tab.Options:
					this.GenerateOptionsMenu();
					break;
				default:
					break;
			}
		}

		private void GenerateTrashListMenu()
		{
			Config.TrashItems
		}

		private void GenerateOptionsMenu()
		{

		}

		private void ResetComponents()
		{
			this.width = 800 + IClickableMenu.borderWidth * 2;
			this.height = 600 + IClickableMenu.borderWidth * 2;
			this.xPositionOnScreen = Game1.uiViewport.Width / 2 - (this.width - (int) (Game1.tileSize * 2.4f)) / 2;
			this.yPositionOnScreen = Game1.uiViewport.Height / 2 - this.height / 2;

			this.Title = new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen, Game1.tileSize * 4, Game1.tileSize), Manifest.Name);

			{
				var i = 0;
				var labelX = (int) (this.xPositionOnScreen - Game1.tileSize * 4.8f);
				var labelY = (int)(this.yPositionOnScreen + Game1.tileSize * 1.5f);
				var labelHeight = (int) (Game1.tileSize * 0.9f);

				this.Tabs.Clear();
				this.Tabs.AddRange(new []
				{
					new ClickableComponent(new Rectangle(labelX, labelY + labelHeight * i++, Game1.tileSize * 5, Game1.tileSize), Tab.TrashList.ToString(), TrashListTabTitle),
					new ClickableComponent(new Rectangle(labelX, labelY + labelHeight * i++, Game1.tileSize * 5, Game1.tileSize), Tab.Options.ToString(), OptionsTabTitle)
				});
			}

			{
				const int scrollbarOffset = Game1.tileSize * 4 / 16;

				this.UpArrow = new ClickableTextureComponent("up-arrow",
					new Rectangle(this.xPositionOnScreen + this.width + scrollbarOffset,
						this.yPositionOnScreen + Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), "", "",
					Game1.mouseCursors, new Rectangle(421, 459, 11, 12), Game1.pixelZoom);

				this.DownArrow = new ClickableTextureComponent("down-arrow",
					new Rectangle(this.xPositionOnScreen + this.width + scrollbarOffset,
						this.yPositionOnScreen + this.height - Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom),
					"", "", Game1.mouseCursors, new Rectangle(421, 472, 11, 12), Game1.pixelZoom);

				this.ScrollBar = new ClickableTextureComponent("scrollbar",
					new Rectangle(this.UpArrow.bounds.X + Game1.pixelZoom * 3,
						this.UpArrow.bounds.Y + this.UpArrow.bounds.Height + Game1.pixelZoom, 6 * Game1.pixelZoom,
						10 * Game1.pixelZoom), "", "", Game1.mouseCursors, new Rectangle(435, 463, 6, 10), Game1.pixelZoom);

				this.ScrollBarRunner = new Rectangle(this.ScrollBar.bounds.X,
					this.UpArrow.bounds.Y + this.UpArrow.bounds.Height + Game1.pixelZoom, this.ScrollBar.bounds.Width,
					this.height - Game1.tileSize * 2 - this.UpArrow.bounds.Height - Game1.pixelZoom * 2);

				this.SetScrollBarToCurrentIndex();
			}

			this.OptionSlots.Clear();
			for (var idx = 0; idx < TrasherMenu.ItemsPerPage; idx++)
			{
				this.OptionSlots.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4, this.yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom + idx * ((this.height - Game1.tileSize * 2) / TrasherMenu.ItemsPerPage), this.width - Game1.tileSize / 2, (this.height - Game1.tileSize * 2) / TrasherMenu.ItemsPerPage + Game1.pixelZoom), String.Concat(idx)));
			}
		}

		private void SetScrollBarToCurrentIndex()
		{
			if (!this.Options.Any()) return;

			if (CurrentItemIndex == this.Options.Count - TrasherMenu.ItemsPerPage)
			{
				this.ScrollBar.bounds.Y =
					this.ScrollBarRunner.Height / Math.Max(1, this.Options.Count - TrasherMenu.ItemsPerPage + 1) *
					this.CurrentItemIndex + this.UpArrow.bounds.Bottom + Game1.pixelZoom;
			}
			else
			{
				this.ScrollBar.bounds.Y = this.DownArrow.bounds.Y - this.ScrollBar.bounds.Height - Game1.pixelZoom;
			}
		}
	}
}
