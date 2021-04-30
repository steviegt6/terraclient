using System.Collections.Generic;

namespace Terraria.Terraclient.Commands
{
	public static class CheatCommandUtils
	{
		public static int ColorIndex { get; set; }

		public static List<string> NonErrorColors = new() {
			"fc7303",
			"8aff9e",
			"8affa7",
			"8affb1",
			"8affbb",
			"8affc5",
			"8affce",
			"8affd8",
			"8affe2",
			"8affeb",
			"8afff5",
			"8affff",
			"fc7303"
		};

		public static List<string> ErrorColors = new() {
			"8a8a8a",
			"f23d2c",
			"eb3b2a",
			"e33929",
			"db3727",
			"d43526",
			"cc3325",
			"c43123",
			"bd2f22",
			"bd2f22",
			"c43123",
			"cc3325",
			"8a8a8a"
		};

		public static void Output(bool isError, string outputText) {
			if (!isError) 
				Main.NewText("[c/fc7303:<]" + 
				             "[c/8aff9e:T]" + 
				             "[c/8affa7:e]" + 
				             "[c/8affb1:r]" + 
				             "[c/8affbb:r]" + 
				             "[c/8affc5:a]" + 
				             "[c/8affce:c]" + 
				             "[c/8affd8:l]" + 
				             "[c/8affe2:i]" + 
				             "[c/8affeb:e]" + 
				             "[c/8afff5:n]" + 
				             "[c/8affff:t]" + 
				             "[c/fc7303:>] " + outputText, 138, 255, 206);
			else
				Main.NewText("[c/8a8a8a:<]" + 
				             "[c/f23d2c:T]" + 
				             "[c/eb3b2a:e]" + 
				             "[c/e33929:r]" + 
				             "[c/db3727:r]" + 
				             "[c/d43526:a]" + 
				             "[c/cc3325:c]" + 
				             "[c/c43123:l]" + 
				             "[c/bd2f22:i]" + 
				             "[c/bd2f22:e]" + 
				             "[c/c43123:n]" + 
				             "[c/cc3325:t]" +
				             "[c/8a8a8a:>] " + outputText, 235, 59, 42);
		}
	}
}
