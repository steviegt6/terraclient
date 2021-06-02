using System.Collections.Generic;
using Terraria.ID;
using Terraria.Localization;

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

				_itemNames[0] = Language.GetTextValue("CustomItemNames.Air");
				_itemNames[4143] = $"{Lang.GetItemNameValue(ItemID.ManaCloak)} {Lang.GetItemNameValue(ItemID.Star)}";
				_itemNames[753] = $"{Lang.GetItemNameValue(ItemID.Seaweed)} ({Lang.GetBuffName(BuffID.PetTurtle)})";
				_itemNames[2338] = $"{Lang.GetItemNameValue(ItemID.FishingSeaweed)} (Fishing garbage)";

				_itemNames[865] =
					$"{Lang.GetItemNameValue(ItemID.PrincessDress)} (From {Lang.GetNPCNameValue(NPCID.Clothier)})";

				_itemNames[1773] = $"{Lang.GetItemNameValue(ItemID.PrincessDressNew)} (From Halloween)";
				_itemNames[3217] = $"{Lang.GetItemNameValue(ItemID.CorruptPlanterBox)} (Corruption)";
				_itemNames[3218] = $"{Lang.GetItemNameValue(ItemID.CrimsonPlanterBox)} (Crimson)";

				_itemNames[3318] =
					$"{Lang.GetItemNameValue(ItemID.KingSlimeBossBag)} ({Lang.GetNPCNameValue(NPCID.KingSlime)})";

				_itemNames[3319] =
					$"{Lang.GetItemNameValue(ItemID.EyeOfCthulhuBossBag)} ({Lang.GetNPCNameValue(NPCID.EyeofCthulhu)})";

				_itemNames[3320] =
					$"{Lang.GetItemNameValue(ItemID.EaterOfWorldsBossBag)} ({Lang.GetNPCNameValue(NPCID.EaterofWorldsHead)})";

				_itemNames[3321] =
					$"{Lang.GetItemNameValue(ItemID.BrainOfCthulhuBossBag)} ({Lang.GetNPCNameValue(NPCID.BrainofCthulhu)})";

				_itemNames[3322] =
					$"{Lang.GetItemNameValue(ItemID.QueenBeeBossBag)} ({Lang.GetNPCNameValue(NPCID.QueenBee)})";

				_itemNames[3323] =
					$"{Lang.GetItemNameValue(ItemID.SkeletronBossBag)} ({Lang.GetNPCNameValue(NPCID.SkeletronHead)})";

				_itemNames[3324] =
					$"{Lang.GetItemNameValue(ItemID.WallOfFleshBossBag)} ({Lang.GetNPCNameValue(NPCID.WallofFlesh)})";

				_itemNames[3325] =
					$"{Lang.GetItemNameValue(ItemID.DestroyerBossBag)} ({Lang.GetNPCNameValue(NPCID.TheDestroyer)})";

				_itemNames[3326] =
					$"{Lang.GetItemNameValue(ItemID.TwinsBossBag)} ({Language.GetTextValue("Enemies.TheTwins")})";

				_itemNames[3327] =
					$"{Lang.GetItemNameValue(ItemID.SkeletronPrimeBossBag)} ({Lang.GetNPCNameValue(NPCID.SkeletronPrime)})";

				_itemNames[3328] =
					$"{Lang.GetItemNameValue(ItemID.PlanteraBossBag)} ({Lang.GetNPCNameValue(NPCID.Plantera)})";

				_itemNames[3329] =
					$"{Lang.GetItemNameValue(ItemID.GolemBossBag)} ({Lang.GetNPCNameValue(NPCID.Golem)})";

				_itemNames[3330] =
					$"{Lang.GetItemNameValue(ItemID.FishronBossBag)} ({Lang.GetNPCNameValue(NPCID.DukeFishron)})";

				_itemNames[3331] =
					$"{Lang.GetItemNameValue(ItemID.CultistBossBag)} ({Lang.GetNPCNameValue(NPCID.CultistBoss)})";

				_itemNames[3332] =
					$"{Lang.GetItemNameValue(ItemID.MoonLordBossBag)} ({Language.GetTextValue("Enemies.MoonLord")})";

				_itemNames[3860] =
					$"{Lang.GetItemNameValue(ItemID.BossBagBetsy)} ({Lang.GetNPCNameValue(NPCID.DD2Betsy)})";

				_itemNames[4782] =
					$"{Lang.GetItemNameValue(ItemID.FairyQueenBossBag)} ({Lang.GetNPCNameValue(NPCID.HallowBoss)})";

				_itemNames[4957] =
					$"{Lang.GetItemNameValue(ItemID.QueenSlimeBossBag)} ({Lang.GetNPCNameValue(NPCID.QueenSlimeBoss)})";

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
				_npcNames[-65] = $"Big {Lang.GetNPCNameValue(NPCID.Hornet)}";
				_npcNames[-64] = $"Little {Lang.GetNPCNameValue(NPCID.Hornet)}";
				_npcNames[-55] = $"Big {Lang.GetNPCNameValue(NPCID.ZombieRaincoat)}";
				_npcNames[-54] = $"Little {Lang.GetNPCNameValue(NPCID.ZombieRaincoat)}";
				_npcNames[-47] = $"Big {Lang.GetNPCNameValue(NPCID.Skeleton)}";
				_npcNames[-46] = $"Little {Lang.GetNPCNameValue(NPCID.Skeleton)}";
				_npcNames[-27] = $"Big {Lang.GetNPCNameValue(NPCID.Zombie)}";
				_npcNames[-26] = $"Little {Lang.GetNPCNameValue(NPCID.Zombie)}";
				_npcNames[-17] = $"Big Weak {Lang.GetNPCNameValue(NPCID.MossHornet)}";
				_npcNames[-16] = $"Little Weak {Lang.GetNPCNameValue(NPCID.MossHornet)}";
				_npcNames[590] = $"Torch {Lang.GetNPCNameValue(NPCID.Zombie)}";
				_npcNames[656] = $"Town {Lang.GetNPCNameValue(NPCID.Bunny)}";
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
				_buffNames[166] = $"{Lang.GetBuffName(BuffID.MinecartRightMech)} (Mechanical)";
				_buffNames[167] = $"{Lang.GetBuffName(BuffID.MinecartLeftMech)} (Mechanical)";
				return _buffNames;
			}
			internal set => _buffNames = value;
		}

		private static Dictionary<int, string> _projNames;

		public static Dictionary<int, string> ProjNames {
			get {
				_projNames ??= new Dictionary<int, string>();

				if (_projNames.Count != 0)
					return _projNames;

				for (int i = 0; i < ProjectileID.Count; i++)
					_projNames.Add(i, Lang.GetProjectileName(i).Value);

				_projNames[0] = "Absence";
				return _projNames;
			}
			internal set => _projNames = value;
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
			string errorTypeName = errorType switch {
				1 => Language.GetTextValue("CommandErrors.InputDidNotCompute"),
				2 => Language.GetTextValue("CommandErrors.>1OptionSelected"),
				3 => Language.GetTextValue("CommandErrors.<1OptionSelected"),
				4 => Language.GetTextValue("CommandErrors.HadAStrokeAndDied"),
				_ => ""
			};

			if (!isError)
				Main.NewText(
					$"[c/fc7303:<][c/8aff9e:T][c/8affa7:e][c/8affb1:r][c/8affbb:r][c/8affc5:a][c/8affce:c][c/8affd8:l][c/8affe2:i][c/8affeb:e][c/8afff5:n][c/8affff:t][c/fc7303:>] {outputText}",
					138, 255, 206);
			else
				Main.NewText(
					$"[c/8a8a8a:<][c/f23d2c:T][c/eb3b2a:e][c/e33929:r][c/db3727:r][c/d43526:a][c/cc3325:c][c/c43123:l][c/bd2f22:i][c/bd2f22:e][c/c43123:n][c/cc3325:t][c/8a8a8a:>] {errorTypeName} Error: {outputText}",
					235, 59, 42);
		}

		public static void ToggleMessage(Cheats.Cheat cheat) {
			string activated = cheat.IsEnabled
				? $"[c/d802f0:{Language.GetTextValue("CommandResponse.ToggleOn")}]"
				: $"[c/d802f0:{Language.GetTextValue("CommandResponse.ToggleOff")}]";
			Output(false, Language.GetTextValue("CommandResponse.ToggleSentence", cheat.Name, activated));
		}
	}
}