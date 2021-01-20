using System.Collections.Generic;

using StardewModdingAPI;

namespace AutoTrasher
{
	internal class ModConfig
	{
		public bool Active { get; set; }
		public bool UseShippingBin { get; set; }
		public SButton ToggleTrasher { get; set; }
		public SButton ToggleShipping { get; set; }
		public SButton AddTrash { get; set; }
		public List<string> TrashItems { get; set; }

		public ModConfig()
		{
			Active = true;
			UseShippingBin = false;

			ToggleTrasher = SButton.R;
			ToggleShipping = SButton.V;
			AddTrash = SButton.X;

			TrashItems = new List<string> { "Broken CD", "Broken Glasses", "Driftwood", "Rotten Plant", "Soggy Newspaper", "Trash" };
		}
	}
}
