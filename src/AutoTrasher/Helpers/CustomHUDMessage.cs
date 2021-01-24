using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace AutoTrasher.Helpers
{
	public class CustomHUDMessage : HUDMessage
	{
		public Item CMessageSubject { get; set; }

		private string _cMessage = "";
		public string CMessage
		{
			get => _cMessage;
			set => _cMessage = Game1.parseText(value, Game1.dialogueFont, 384);
		}

		#region Base HUD Constructors

		public CustomHUDMessage(string message) : base(message)
		{
		}

		public CustomHUDMessage(string message, bool achievement) : base(message, achievement)
		{
		}

		public CustomHUDMessage(string message, int whatType) : base(message, whatType)
		{
		}

		public CustomHUDMessage(string type, int number, bool add, Color color, Item messageSubject = null) : base(type, number, add, color, messageSubject)
		{
			CMessageSubject = messageSubject;
		}

		public CustomHUDMessage(string message, Color color, float timeLeft) : base(message, color, timeLeft)
		{
		}

		public CustomHUDMessage(string message, string leaveMeNull) : base(message, leaveMeNull)
		{
		}

		public CustomHUDMessage(string message, Color color, float timeLeft, bool fadeIn) : base(message, color, timeLeft, fadeIn)
		{
		}

		#endregion

		public override void draw(SpriteBatch b, int i)
		{
			var titleSafeArea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();

			if (noIcon)
			{
				var overrideX = titleSafeArea.Left + 16;
				var overrideY = (Game1.uiViewport.Width < 1400 ? -64 : 0) + titleSafeArea.Bottom - (i + 1) * 64 * 7 / 4 - 21 - (int)Game1.dialogueFont.MeasureString(CMessage).Y;

				IClickableMenu.drawHoverText(b, CMessage, Game1.dialogueFont, overrideX: overrideX, overrideY: overrideY, alpha: transparency);
			}
			else
			{
				var vector2 = new Vector2(titleSafeArea.Left + 16, titleSafeArea.Bottom - (i + 1) * 64 * 7 / 4 - 64);

				if (Game1.isOutdoorMapSmallerThanViewport())
				{
					vector2.X = Math.Max(titleSafeArea.Left + 16, -Game1.uiViewport.X + 16);
				}

				if (Game1.uiViewport.Width < 1400)
				{
					vector2.Y -= 48f;
				}

				// Normal or Purple background for Icon
				// ###jag $NOTE - Changed the rectangle to only include the icon square and NOT the start of the text box
				b.Draw(Game1.mouseCursors, vector2, !(CMessageSubject is Object) || ((Object) CMessageSubject).sellToStorePrice() <= 500 ? new Rectangle(293, 360, 23, 24) : new Rectangle(163, 399, 23, 24), Color.White * transparency, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

				// this section is temp removed due try and make my own text box
				{
					//var x = Game1.smallFont.MeasureString(CMessage ?? "").X;

					// Body tile for text box
					//b.Draw(Game1.mouseCursors, new Vector2(vector2.X + 104f, vector2.Y), new Rectangle(319, 360, 1, 24), Color.White * transparency, 0.0f, Vector2.Zero, new Vector2(x, 4f), SpriteEffects.None, 1f);

					// Rounded end tile for text box
					//b.Draw(Game1.mouseCursors, new Vector2(vector2.X + 104f + x, vector2.Y), new Rectangle(323, 360, 6, 24), Color.White * transparency, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
				}

				// Set up vector start location for drawing ICONS w/i icon background box
				vector2.X += 16f;
				vector2.Y += 16f;

				// Draw ICONS
				if (CMessageSubject == null)
				{
					switch (whatType)
					{
						case 1:
							b.Draw(Game1.mouseCursors, vector2 + new Vector2(8f, 8f) * 4f, new Rectangle(294, 392, 16, 16), Color.White * transparency, 0.0f, new Vector2(8f, 8f), 4f + Math.Max(0.0f, (float)((timeLeft - 3000.0) / 900.0)), SpriteEffects.None, 1f);
							break;
						case 2:
							b.Draw(Game1.mouseCursors, vector2 + new Vector2(8f, 8f) * 4f, new Rectangle(403, 496, 5, 14), Color.White * transparency, 0.0f, new Vector2(3f, 7f), 4f + Math.Max(0.0f, (float)((timeLeft - 3000.0) / 900.0)), SpriteEffects.None, 1f);
							break;
						case 3:
							b.Draw(Game1.mouseCursors, vector2 + new Vector2(8f, 8f) * 4f, new Rectangle(268, 470, 16, 16), Color.White * transparency, 0.0f, new Vector2(8f, 8f), 4f + Math.Max(0.0f, (float)((timeLeft - 3000.0) / 900.0)), SpriteEffects.None, 1f);
							break;
						case 4:
							b.Draw(Game1.mouseCursors, vector2 + new Vector2(8f, 8f) * 4f, new Rectangle(0, 411, 16, 16), Color.White * transparency, 0.0f, new Vector2(8f, 8f), 4f + Math.Max(0.0f, (float)((timeLeft - 3000.0) / 900.0)), SpriteEffects.None, 1f);
							break;
						case 5:
							b.Draw(Game1.mouseCursors, vector2 + new Vector2(8f, 8f) * 4f, new Rectangle(16, 411, 16, 16), Color.White * transparency, 0.0f, new Vector2(8f, 8f), 4f + Math.Max(0.0f, (float)((timeLeft - 3000.0) / 900.0)), SpriteEffects.None, 1f);
							break;
						case 6:
							b.Draw(Game1.mouseCursors2, vector2 + new Vector2(8f, 8f) * 4f, new Rectangle(96, 32, 16, 16), Color.White * transparency, 0.0f, new Vector2(8f, 8f), 4f + Math.Max(0.0f, (float)((timeLeft - 3000.0) / 900.0)), SpriteEffects.None, 1f);
							break;
					}
				}
				else
				{
					CMessageSubject.drawInMenu(b, vector2, 1f + Math.Max(0.0f, (float)((timeLeft - 3000.0) / 900.0)), transparency, 1f, StackDrawType.Hide);
				}

				// Move vector to tiny digit location
				vector2.X += 51f;
				vector2.Y += 51f;

				// Move vector to text box starting point
				vector2.X += 16f;

				switch (Game1.dialogueFont.MeasureString(CMessage).Y)
				{
					case 51: // single line of text
						vector2.Y -= 65f;
						break;
					case 93: // two lines of text
						vector2.Y -= 86f;
						break;
					case 135: // three lines of text
						vector2.Y -= 106f;
						break;
					default: // four lines, any more than that is just ridiculous
						vector2.Y -= 123f;
						break;
				}

				Utilities.DrawHoverText(b, CMessage, Game1.dialogueFont, overrideX: (int) vector2.X, overrideY: (int) vector2.Y, alpha: transparency, drawBoxShadows: false);
			}
		}
	}
}
