using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace AutoTrasher.Helpers
{
	public class Utilities
	{
		// Based on StardewValley.Menus.IClickableMenu.cs
		// Altered to
		//	- Be able to add / remove shadows for text box
		//	- Adjust a specific box width
		public static void DrawHoverText(
			SpriteBatch b,
			string text,
			SpriteFont font,
			int xOffset = 0,
			int yOffset = 0,
			int moneyAmountToDisplayAtBottom = -1,
			string boldTitleText = null,
			int healAmountToDisplay = -1,
			string[] buffIconsToDisplay = null,
			Item hoveredItem = null,
			int currencySymbol = 0,
			int extraItemToShowIndex = -1,
			int extraItemToShowAmount = -1,
			int overrideX = -1,
			int overrideY = -1,
			float alpha = 1f,
			CraftingRecipe craftingIngredients = null,
			IList<Item> additionalCraftMaterials = null,
			bool drawBoxShadows = true)
		{
			var sbText = (StringBuilder) null;
			if (text != null)
			{
				sbText = new StringBuilder(text);
			}

			DrawHoverText(b, sbText, font, xOffset, yOffset, moneyAmountToDisplayAtBottom, boldTitleText, healAmountToDisplay, buffIconsToDisplay, hoveredItem, currencySymbol, extraItemToShowIndex, extraItemToShowAmount, overrideX, overrideY, alpha, craftingIngredients, additionalCraftMaterials, drawBoxShadows);
		}

		// Based on StardewValley.Menus.IClickableMenu.cs
		// Altered to add / remove shadows for text box
		public static void DrawHoverText(
			SpriteBatch b,
			StringBuilder text,
			SpriteFont font,
			int xOffset = 0,
			int yOffset = 0,
			int moneyAmountToDisplayAtBottom = -1,
			string boldTitleText = null,
			int healAmountToDisplay = -1,
			string[] buffIconsToDisplay = null,
			Item hoveredItem = null,
			int currencySymbol = 0,
			int extraItemToShowIndex = -1,
			int extraItemToShowAmount = -1,
			int overrideX = -1,
			int overrideY = -1,
			float alpha = 1f,
			CraftingRecipe craftingIngredients = null,
			IList<Item> additionalCraftMaterials = null,
			bool drawBoxShadows = true)
		{
			if (text == null || text.Length == 0) return;

			var text1 = (string) null;
			boldTitleText = boldTitleText == "" ? null : boldTitleText;

			var num1 = Math.Max(
				healAmountToDisplay != -1 ? (int) font.MeasureString(healAmountToDisplay + "+ Energy" + 32).X : 0,
				Math.Max((int) font.MeasureString(text).X,
					boldTitleText != null
						? (int) (Game1.dialogueFont.MeasureString(boldTitleText).Y + 16.0)
						: 0));

			var height = Math.Max(20 * 3,
				(int) font.MeasureString(text).Y + 32 +
				(moneyAmountToDisplayAtBottom > -1
					? (int) (font.MeasureString(string.Concat(moneyAmountToDisplayAtBottom)).Y + 4.0)
					: 8) + (boldTitleText != null
					? (int) (Game1.dialogueFont.MeasureString(boldTitleText).Y + 16.0)
					: 0));

			if (extraItemToShowAmount != -1)
			{
				var strArray = Game1.objectInformation[extraItemToShowIndex].Split('/');
				var word = strArray[0];

				if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
				{
					word = strArray[4];
				}

				var text2 = Game1.content.LoadString("Strings\\UI:ItemHover_Requirements",
					extraItemToShowAmount,
					extraItemToShowAmount > 1 ? Lexicon.makePlural(word) : word);

				var num2 = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, extraItemToShowIndex, 16, 16).Width * 2 * 4;
				num1 = Math.Max(num1, num2 + (int) font.MeasureString(text2).X);
			}

			if (buffIconsToDisplay != null)
			{
				height += buffIconsToDisplay.Where(str => !str.Equals("0")).Sum(str => 34);
				height += 4;
			}

			if (craftingIngredients != null && Game1.options.showAdvancedCraftingInformation && craftingIngredients.getCraftCountText() != null)
			{
				height += (int) font.MeasureString("T").Y;
			}

			var text3 = (string) null;

			if (hoveredItem != null)
			{
				var startingHeight = height + 68 * hoveredItem.attachmentSlots();
				text3 = hoveredItem.getCategoryName();

				if (text3.Length > 0)
				{
					num1 = Math.Max(num1, (int) font.MeasureString(text3).X + 32);
					startingHeight += (int) font.MeasureString("T").Y;
				}

				const int num2 = 9999;
				const int horizontalBuffer = 92;
				var tooltipSpecialIcons = hoveredItem.getExtraSpaceNeededForTooltipSpecialIcons(font, num1,
					horizontalBuffer, startingHeight, text, boldTitleText, moneyAmountToDisplayAtBottom);

				num1 = tooltipSpecialIcons.X != 0 ? tooltipSpecialIcons.X : num1;
				height = tooltipSpecialIcons.Y != 0 ? tooltipSpecialIcons.Y : startingHeight;

				if (hoveredItem is MeleeWeapon weapon && weapon.GetTotalForgeLevels() > 0)
				{
					height += (int) font.MeasureString("T").Y;
				}

				switch (hoveredItem)
				{
					case MeleeWeapon meleeWeapon when meleeWeapon.GetEnchantmentLevel<GalaxySoulEnchantment>() > 0:
						height += (int) font.MeasureString("T").Y;
						break;
					case Object obj when obj.Edibility != -300:
					{
						if (healAmountToDisplay != -1)
						{
							height += 40 * (healAmountToDisplay > 0 ? 2 : 1);
						}
						else
						{
							height += 40;
						}

						healAmountToDisplay = obj.staminaRecoveredOnConsumption();
						num1 = (int) Math.Max(num1,
							Math.Max(
								font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Energy",
									num2)).X + horizontalBuffer,
								font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Health",
									num2)).X + horizontalBuffer));
						break;
					}
				}

				if (buffIconsToDisplay != null)
				{
					for (var index = 0; index < buffIconsToDisplay.Length; ++index)
					{
						if (!buffIconsToDisplay[index].Equals("0") && index <= 11)
						{
							num1 = (int) Math.Max(num1,
								font.MeasureString(
									Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + index,
										num2)).X + horizontalBuffer);
						}
					}
				}
			}

			var vector21 = Vector2.Zero;

			if (craftingIngredients != null)
			{
				if (Game1.options.showAdvancedCraftingInformation)
				{
					var craftableCount = craftingIngredients.getCraftableCount(additionalCraftMaterials);

					if (craftableCount > 1)
					{
						text1 = " (" + craftableCount + ")";
						vector21 = Game1.smallFont.MeasureString(text1);
					}
				}

				num1 = (int) Math.Max((float) (Game1.dialogueFont.MeasureString(boldTitleText).X + vector21.X + 12.0), 384f);
				height += craftingIngredients.getDescriptionHeight(num1 - 8) + (healAmountToDisplay == -1 ? -32 : 0);
			}

			var x = Game1.getOldMouseX() + 32 + xOffset;
			var y1 = Game1.getOldMouseY() + 32 + yOffset;

			if (overrideX != -1)
			{
				x = overrideX;
			}

			if (overrideY != -1)
			{
				y1 = overrideY;
			}

			var num3 = x + num1;
			var safeArea = Utility.getSafeArea();
			var right1 = safeArea.Right;

			if (num3 > right1)
			{
				safeArea = Utility.getSafeArea();
				x = safeArea.Right - num1;
				y1 += 16;
			}

			var num4 = y1 + height;
			safeArea = Utility.getSafeArea();
			var bottom = safeArea.Bottom;

			if (num4 > bottom)
			{
				x += 16;
				var num2 = x + num1;
				safeArea = Utility.getSafeArea();
				var right2 = safeArea.Right;

				if (num2 > right2)
				{
					safeArea = Utility.getSafeArea();
					x = safeArea.Right - num1;
				}

				safeArea = Utility.getSafeArea();
				y1 = safeArea.Bottom - height;
			}

			IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y1, num1 + 21, height, Color.White * alpha, drawShadow: drawBoxShadows);

			if (boldTitleText != null)
			{
				var vector22 = Game1.dialogueFont.MeasureString(boldTitleText);

				IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y1,
					num1 + (craftingIngredients != null ? 21 : 0),
					(int) Game1.dialogueFont.MeasureString(boldTitleText).Y + 32 +
					(hoveredItem == null || text3.Length <= 0 ? 0 : (int) font.MeasureString("asd").Y) - 4,
					Color.White * alpha, drawShadow: false);

				b.Draw(Game1.menuTexture,
					new Rectangle(x + 12,
						y1 + (int) Game1.dialogueFont.MeasureString(boldTitleText).Y + 32 +
						(hoveredItem == null || text3.Length <= 0 ? 0 : (int) font.MeasureString("asd").Y) - 4,
						num1 - 4 * (craftingIngredients == null ? 6 : 1), 4), new Rectangle(44, 300, 4, 4),
					Color.White);

				b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, y1 + 16 + 4) + new Vector2(2f, 2f), Game1.textShadowColor);
				b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, y1 + 16 + 4) + new Vector2(0.0f, 2f), Game1.textShadowColor);
				b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, y1 + 16 + 4), Game1.textShadowColor);

				if (text1 != null)
				{
					Utility.drawTextWithShadow(b, text1, Game1.smallFont, new Vector2(x + 16 + vector22.X, (int) (y1 + 16 + 4 + vector22.Y / 2.0 - vector21.Y / 2.0)), Game1.textShadowColor);
				}

				y1 += (int) Game1.dialogueFont.MeasureString(boldTitleText).Y;
			}

			int y2;

			if (hoveredItem != null && text3.Length > 0)
			{
				var num2 = y1 - 4;
				Utility.drawTextWithShadow(b, text3, font, new Vector2(x + 16, num2 + 16 + 4), hoveredItem.getCategoryColor(), horizontalShadowOffset: 2, verticalShadowOffset: 2);
				y2 = num2 + (int) font.MeasureString("T").Y + (boldTitleText != null ? 16 : 0) + 4;

				if (hoveredItem is Tool tool && tool.GetTotalForgeLevels() > 0)
				{
					var text2 = Game1.content.LoadString("Strings\\UI:Item_Tooltip_Forged");
					Utility.drawTextWithShadow(b, text2, font, new Vector2(x + 16, y2 + 16 + 4), Color.DarkRed, horizontalShadowOffset: 2, verticalShadowOffset: 2);
					var totalForgeLevels = tool.GetTotalForgeLevels();

					if (totalForgeLevels < tool.GetMaxForges() && !tool.hasEnchantmentOfType<DiamondEnchantment>())
					{
						Utility.drawTextWithShadow(b, " (" + totalForgeLevels + "/" + tool.GetMaxForges() + ")", font, new Vector2(x + 16 + font.MeasureString(text2).X, y2 + 16 + 4), Color.DimGray, horizontalShadowOffset: 2, verticalShadowOffset: 2);
					}

					y2 += (int) font.MeasureString("T").Y;
				}

				if (hoveredItem is MeleeWeapon meleeWeapon && meleeWeapon.GetEnchantmentLevel<GalaxySoulEnchantment>() > 0)
				{
					var enchantmentOfType = meleeWeapon.GetEnchantmentOfType<GalaxySoulEnchantment>();
					var text2 = Game1.content.LoadString("Strings\\UI:Item_Tooltip_GalaxyForged");

					Utility.drawTextWithShadow(b, text2, font, new Vector2(x + 16, y2 + 16 + 4), Color.DarkRed, horizontalShadowOffset: 2, verticalShadowOffset: 2);

					var level = enchantmentOfType.GetLevel();
					if (level < enchantmentOfType.GetMaximumLevel())
					{
						Utility.drawTextWithShadow(b, " (" + level + "/" + enchantmentOfType.GetMaximumLevel() + ")", font, new Vector2(x + 16 + font.MeasureString(text2).X, y2 + 16 + 4), Color.DimGray, horizontalShadowOffset: 2, verticalShadowOffset:2);
					}

					y2 += (int) font.MeasureString("T").Y;
				}
			}
			else
			{
				y2 = y1 + (boldTitleText != null ? 16 : 0);
			}

			if (hoveredItem != null && craftingIngredients == null)
			{
				hoveredItem.drawTooltip(b, ref x, ref y2, font, alpha, text);
			}
			else if (text != null && text.Length != 0 && (text.Length != 1 || text[0] != ' '))
			{
				b.DrawString(font, text, new Vector2(x + 16, y2 + 16 + 4) + new Vector2(2f, 2f), Game1.textShadowColor * alpha);
				b.DrawString(font, text, new Vector2(x + 16, y2 + 16 + 4) + new Vector2(0.0f, 2f), Game1.textShadowColor * alpha);
				b.DrawString(font, text, new Vector2(x + 16, y2 + 16 + 4) + new Vector2(2f, 0.0f), Game1.textShadowColor * alpha);
				b.DrawString(font, text, new Vector2(x + 16, y2 + 16 + 4), Game1.textColor * 0.9f * alpha);

				y2 += (int) font.MeasureString(text).Y + 4;
			}

			if (craftingIngredients != null)
			{
				craftingIngredients.drawRecipeDescription(b, new Vector2(x + 16, y2 -8), num1, additionalCraftMaterials);
				y2 += craftingIngredients.getDescriptionHeight(num1 - 8);
			}

			if (healAmountToDisplay != -1)
			{
				var num2 = (hoveredItem as Object).staminaRecoveredOnConsumption();
				if (num2 >= 0)
				{
					var num5 = (hoveredItem as Object).healthRecoveredOnConsumption();
					Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(x + 16 + 4, y2 + 16), new Rectangle(0, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 3f, layerDepth: 0.95f);
					Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Energy", (num2 > 0 ? "+" : "") + num2), font, new Vector2(x + 16 + 34 + 4, y2 + 16), Game1.textColor);
					y2 += 34;

					if (num5 > 0)
					{
						Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(x + 16 + 4, y2 + 16), new Rectangle(0, 438, 10, 10), Color.White, 0.0f, Vector2.Zero, 3f, layerDepth: 0.95f);
						Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Health", "+" + num5), font, new Vector2(x + 16 + 34 + 4, y2 + 16), Game1.textColor);
						y2 += 34;
					}
				}
				else if (num2 != -300)
				{
					Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(x + 16 + 4, y2 + 16), new Rectangle(140, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 3f, layerDepth: 0.95f);
					Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Energy", string.Concat(num2)), font, new Vector2(x + 16 + 34 + 4, y2 + 16), Game1.textColor);
					y2 += 34;
				}
			}

			if (buffIconsToDisplay != null)
			{
				for (var index = 0; index < buffIconsToDisplay.Length; ++index)
				{
					if (!buffIconsToDisplay[index].Equals("0"))
					{
						Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(x + 16 + 4, y2 + 16), new Rectangle(10 + index * 10, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 3f, layerDepth: 0.95f);
						var text2 = (Convert.ToInt32(buffIconsToDisplay[index]) > 0 ? "+" : "") + buffIconsToDisplay[index] + " ";

						if (index <= 11)
						{
							text2 = Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + index, text2);
						}

						Utility.drawTextWithShadow(b, text2, font, new Vector2(x + 16 + 34 + 4, y2 + 16), Game1.textColor);
						y2 += 34;
					}
				}
			}

			if (hoveredItem != null && hoveredItem.attachmentSlots() > 0)
			{
				hoveredItem.drawAttachments(b, x + 16, y2 + 16);

				if (moneyAmountToDisplayAtBottom > -1)
				{
					y2 += 68 * hoveredItem.attachmentSlots();
				}
			}

			if (moneyAmountToDisplayAtBottom > -1)
			{
				b.DrawString(font, string.Concat(moneyAmountToDisplayAtBottom), new Vector2(x + 16, y2 + 16 + 4) + new Vector2(2f, 2f), Game1.textShadowColor);
				b.DrawString(font, string.Concat(moneyAmountToDisplayAtBottom), new Vector2(x + 16, y2 + 16 + 4) + new Vector2(0.0f, 2f), Game1.textShadowColor);
				b.DrawString(font, string.Concat(moneyAmountToDisplayAtBottom), new Vector2(x + 16, y2 + 16 + 4) + new Vector2(2f, 0.0f), Game1.textShadowColor);
				b.DrawString(font, string.Concat(moneyAmountToDisplayAtBottom), new Vector2(x + 16, y2 + 16 + 4), Game1.textColor);

				switch (currencySymbol)
				{
					case 0:
						b.Draw(Game1.debrisSpriteSheet, new Vector2((float) ((double) x + 16 + font.MeasureString(string.Concat(moneyAmountToDisplayAtBottom)).X + 20.0), y2 + 16 + 16), Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, 8, 16, 16), Color.White, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 0.95f);
						break;
					case 1:
						b.Draw(Game1.mouseCursors, new Vector2((float) (x + 8 + (double) font.MeasureString(string.Concat(moneyAmountToDisplayAtBottom)).X + 20.0), y2 + 16 - 5), new Rectangle(338, 400, 8, 8), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
						break;
					case 2:
						b.Draw(Game1.mouseCursors, new Vector2((float) (x + 8 + (double) font.MeasureString(string.Concat(moneyAmountToDisplayAtBottom)).X + 20.0), y2 + 16 - 7), new Rectangle(211, 373, 9, 10), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
						break;
					case 4:
						b.Draw(Game1.objectSpriteSheet, new Vector2((float) (x + 8 + (double) font.MeasureString(string.Concat(moneyAmountToDisplayAtBottom)).X + 20.0), y2 + 16 - 7), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 858, 16, 16), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
						break;
				}

				y2 += 48;
			}

			if (extraItemToShowIndex != -1)
			{
				if (moneyAmountToDisplayAtBottom == -1)
				{
					y2 += 8;
				}

				var str = Game1.objectInformation[extraItemToShowIndex].Split('/')[4];
				var text2 = Game1.content.LoadString("Strings\\UI:ItemHover_Requirements", extraItemToShowAmount, str);
				var num2 = Math.Max(font.MeasureString(text2).Y + 21f, 96f);

				IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y2 + 4, num1 + (craftingIngredients != null ? 21 : 0), (int) num2, Color.White, drawShadow: drawBoxShadows);

				y2 += 20;
				b.DrawString(font, text2, new Vector2(x + 16, y2 + 4) + new Vector2(2f, 2f), Game1.textShadowColor);
				b.DrawString(font, text2, new Vector2(x + 16, y2 + 4) + new Vector2(0.0f, 2f), Game1.textShadowColor);
				b.DrawString(font, text2, new Vector2(x + 16, y2 + 4) + new Vector2(2f, 0.0f), Game1.textShadowColor);
				b.DrawString(Game1.smallFont, text2, new Vector2(x + 16, y2 + 4), Game1.textColor);
				b.Draw(Game1.objectSpriteSheet, new Vector2(x + 16 + (int) font.MeasureString(text2).X + 21, y2), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, extraItemToShowIndex, 16, 16), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
			}

			if (craftingIngredients == null || !Game1.options.showAdvancedCraftingInformation) return;

			Utility.drawTextWithShadow(b, craftingIngredients.getCraftCountText(), font, new Vector2(x + 16, y2 + 16 + 4), Game1.textColor, horizontalShadowOffset: 2, verticalShadowOffset: 2);
		}
	}
}
