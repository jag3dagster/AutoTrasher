using Microsoft.Xna.Framework;
using StardewValley;

namespace AutoTrasher.Helpers
{
	public class CustomHUDMessage : HUDMessage
	{
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
	}
}
