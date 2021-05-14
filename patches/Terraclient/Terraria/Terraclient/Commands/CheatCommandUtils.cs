using System.Collections.Generic;
using Terraria.ID;

namespace Terraria.Terraclient.Commands
{
	public static class CheatCommandUtils
	{
		public static int ColorIndex { get; set; }

		private static Dictionary<int, string> _itemNames;

		public static Dictionary<int, string> ItemNames {
			get {
				_itemNames ??= new Dictionary<int, string>();

				if (_itemNames.Count != 0)
					return _itemNames;

				for (int i = 0; i < ItemID.Count; i++)
					_itemNames.Add(i, Lang.GetItemNameValue(i));

				_itemNames[0] = "Air";
				_itemNames[4143] = "Mana Cloak Star";
				_itemNames[753] = "Seaweed (Turtle pet)";
				_itemNames[2338] = "Seaweed (Fishing garbage)";
				_itemNames[865] = "Princess dress (From clothier)";
				_itemNames[1773] = "Princess dress (From halloween)";
				_itemNames[3217] = "Deathweed Planter Box (Corruption)";
				_itemNames[3218] = "Deathweed Planter Box (Crimson)";
				_itemNames[3318] = "Treasure Bag (King Slime)";
				_itemNames[3319] = "Treasure Bag (Eye of Cthulhu)";
				_itemNames[3320] = "Treasure Bag (Eater of Worlds)";
				_itemNames[3321] = "Treasure Bag (Brain of Cthulhu)";
				_itemNames[3322] = "Treasure Bag (Queen Bee)";
				_itemNames[3323] = "Treasure Bag (Skeletron)";
				_itemNames[3324] = "Treasure Bag (Wall of Flesh)";
				_itemNames[3325] = "Treasure Bag (Destroyer)";
				_itemNames[3326] = "Treasure Bag (Twins)";
				_itemNames[3327] = "Treasure Bag (Skeletron Prime)";
				_itemNames[3328] = "Treasure Bag (Plantera)";
				_itemNames[3329] = "Treasure Bag (Golem)";
				_itemNames[3330] = "Treasure Bag (Fishron)";
				_itemNames[3331] = "Treasure Bag (Cultist)";
				_itemNames[3332] = "Treasure Bag (Moon Lord)";
				_itemNames[3860] = "Treasure Bag (Betsy)";
				_itemNames[4782] = "Treasure Bag (Fairy Queen)";
				_itemNames[4957] = "Treasure Bag (Queen Slime)";
				//_itemNames[3386] = "(Variant 2) Strange Plant";
				//_itemNames[3387] = "(Variant 3) Strange Plant";
				//_itemNames[3388] = "(Variant 4) Strange Plant";
				//Just designate these other plants in-game via item ID.
				return _itemNames;
			}
			internal set => _itemNames = value;
		}

		private static Dictionary<int, string> _npcNames;

		public static Dictionary<int, string> NPCNames {
			get {
				_npcNames ??= new Dictionary<int, string>();

				if (_npcNames.Count != 0)
					return _npcNames;

				for (int i = -65; i < NPCID.Count; i++)
					_npcNames.Add(i, Lang.GetNPCNameValue(i));

				_npcNames[0] = "Nothing";
				_npcNames[-65] = "Big Hornet";
				_npcNames[-64] = "Little Hornet";
				_npcNames[-55] = "Big Raincoat Zombie";
				_npcNames[-54] = "Little Raincoat Zombie";
				_npcNames[-47] = "Big Skeleton";
				_npcNames[-46] = "Little Skeleton";
				_npcNames[-27] = "Big Zombie";
				_npcNames[-26] = "Little Zombie";
				_npcNames[-17] = "Big Weak Hornet";
				_npcNames[-16] = "Little Weak Hornet";
				_npcNames[590] = "Torch Zombie";
				_npcNames[656] = "Town Bunny";
				return _npcNames;
			}
			internal set => _npcNames = value;
		}

		private static Dictionary<int, string> _buffNames;

		public static Dictionary<int, string> BuffNames {
			get {
				_buffNames ??= new Dictionary<int, string>();

				if (_buffNames.Count != 0)
					return _buffNames;

				for (int i = 0; i < BuffID.Count; i++)
					_buffNames.Add(i, Lang.GetBuffName(i));

				_buffNames[0] = "Nothing";
				_buffNames[166] = "Minecart (Mechanical)";
				_buffNames[167] = "Minecart (Mechanical)";
				return _buffNames;
			}
			internal set => _buffNames = value;
		}

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

		public static void Output(bool isError, string outputText, int errorType = 0) {
			string errorTypeName = "";
			if (errorType == 0)
				errorTypeName = "";
			else if (errorType == 1)
				errorTypeName = "Syntax"; //Command entered incorrectly
			else if (errorType == 2)
				errorTypeName =
					"Indecision"; //Command stopped at indecision between two options in autocomplete selection or otherwise
			else if (errorType == 3)
				errorTypeName = "Not an Option"; //There's no thing corresponding to thingID
			else if (errorType == 4)
				errorTypeName = "Fatal"; //Literally broke
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
				             "[c/8a8a8a:>] " + errorTypeName + " Error: " + outputText, 235, 59, 42);
		}

		public static void ToggleMessage(Cheats.Cheat cheat) {
			string activated = "[c/d4f01f:Activated]";
			if (!cheat.isEnabled) {
				activated = "[c/f0421f:Deactivated]";
			}
			Output(false, string.Format("{0} {1}.", cheat.Name, activated));
		}
	}
}