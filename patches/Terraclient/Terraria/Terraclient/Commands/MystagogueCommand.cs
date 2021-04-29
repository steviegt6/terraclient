using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Terraria.ID;

namespace Terraria.Terraclient.Commands
{
	public class MystagogueCommand
	{
		public MystagogueCommand(string commandName, string commandDescription, Action<List<string>> commandAction) {
			this.commandName = commandName;
			this.commandDescription = commandDescription;
			this.commandAction = commandAction;
			MystagogueCommand.commandList.Add(this);
		}

		static MystagogueCommand() {
			new MystagogueCommand("help", "Returns \"test\" in chat.", delegate (List<string> args) {
				Main.NewText("test", 255, 255, 255);
			});
			new MystagogueCommand("i", "(Name-Concatenated/ID, Stack, Prefix) Spawns an item by converting your currently held cursor item (or thin air) to it.", delegate (List<string> args) {
				if (args.Count == 0) {
					Main.NewText("That command requires arguments", 255, 0, 0);
					return;
				}
				int finalItemSelection = 0;
				if (!new Regex("\\D").IsMatch(args[0])) {
					string text = args[0];
					while (text.StartsWith("0")) {
						text = text.Remove(0, 1);
					}
					while (text != finalItemSelection.ToString()) {
						if (finalItemSelection == ItemID.Count) {
							Main.NewText("Given item ID does not correspond to an item", 255, 0, 0);
							return;
						}
						finalItemSelection++;
					}
					args.RemoveAt(0);
				}
				else {
					int indexOfStackCountArg = 1;
					while (new Regex("\\D").IsMatch(args[indexOfStackCountArg])) {
						indexOfStackCountArg++;
						if (indexOfStackCountArg == args.Count) {
							break;
						}
					}
					string fullItemNameQuery = string.Join(" ", args.GetRange(0, indexOfStackCountArg)).ToLower();
					args = args.GetRange(indexOfStackCountArg, args.Count - indexOfStackCountArg);
					List<int> foundItems = new List<int>();
					for (int i = 0; i < ItemID.Count; i++) {
						if (Lang.GetItemNameValue(i).ToLower().StartsWith(fullItemNameQuery)) {
							foundItems.Add(i);
						}
					}
					if (foundItems.Count == 0) {
						Main.NewText("No item names match", 255, 0, 0);
						return;
					}
					if (foundItems.Count > 1) {
						List<string> itemNameListToCheck = new List<string>();
						foreach (int id in foundItems) {
							itemNameListToCheck.Add(Lang.GetItemNameValue(id).ToLower());
						}
						List<string> itemNameAndIDsToOutput = new List<string>();
						foreach (int id in foundItems) {
							itemNameAndIDsToOutput.Add(string.Concat(new object[]
							{
								Lang.GetItemNameValue(id),
								" (",
								id,
								")"
							}));
						}
						for (int k = 0; k < itemNameListToCheck.Count; k++) {
							if (itemNameListToCheck[k] == fullItemNameQuery) {
								finalItemSelection = foundItems[k];
								itemNameAndIDsToOutput.RemoveAt(k);
								Main.NewText("Other matches include " + string.Join(", ", itemNameAndIDsToOutput), 255, 255, 255);
								break;
							}
							if (k + 1 == foundItems.Count) {
								Main.NewText("Query too unspecific, found " + string.Join(", ", itemNameAndIDsToOutput), 255, 0, 0);
								return;
							}
						}
					}
					else {
						finalItemSelection = foundItems[0];
					}
				}
				int stack = 1;
				int num4 = 0;
				if (args.Count >= 1) {
					if (new Regex("\\D").IsMatch(args[0])) {
						Main.NewText("Stack must be a positive integer", 255, 0, 0);
						return;
					}
					string parsingString = args[0];
					while (parsingString.StartsWith("0")) {
						parsingString = parsingString.Remove(0, 1);
					}
					if (parsingString.Length > 10) {
						stack = int.MaxValue;
					}
					else if (Convert.ToInt64(parsingString) > 2147483647L) {
						stack = int.MaxValue;
					}
					else if (parsingString.Length > 0) {
						stack = int.Parse(parsingString);
					}
				}
				int finalPrefixSelection = 0;
				if (args.Count >= 2) {
					if (!new Regex("\\D").IsMatch(args[1])) {
						string parsingString = args[1];
						while (parsingString.StartsWith("0")) {
							parsingString = parsingString.Remove(0, 1);
						}
						while (parsingString != finalPrefixSelection.ToString()) {
							if (finalPrefixSelection == PrefixID.Count) {
								Main.NewText("Given prefix ID does not correspond to a prefix", 255, 0, 0);
								return;
							}
							finalPrefixSelection++;
						}
					}
					else {
						string parsingString = args[1].ToLower();
						List<int> foundPrefixes = new List<int>();
						for (int i = 0; i < MystagogueCommand.Prefixes.Length; i++) {
							if (MystagogueCommand.Prefixes[i].ToLower().StartsWith(parsingString) && i != 75 && i != 43 && i != 76) {
								foundPrefixes.Add(i);
							}
						}
						if (foundPrefixes.Count == 0) {
							Main.NewText("No prefix names match", 255, 0, 0);
							return;
						}
						if (foundPrefixes.Count > 1) {
							List<string> prefixNamesAndIDsToOutput = new List<string>();
							foreach (int i in foundPrefixes) {
								prefixNamesAndIDsToOutput.Add(string.Concat(new object[]
								{
									MystagogueCommand.Prefixes[i],
									" (",
									i,
									")"
								}));
							}
							Main.NewText("Prefix query too unspecific, found " + string.Join(", ", prefixNamesAndIDsToOutput), 255, 0, 0);
							return;
						}
						finalPrefixSelection = foundPrefixes[0];
					}
					if (finalPrefixSelection == 18 || finalPrefixSelection == 75) {
						if (!ContentSamples.ItemsByType[finalItemSelection].accessory) {
							finalPrefixSelection = 18;
						}
						else {
							finalPrefixSelection = 75;
						}
					}
					if (finalPrefixSelection == 20 || finalPrefixSelection == 43) {
						if (ContentSamples.ItemsByType[finalItemSelection].ranged) {
							finalPrefixSelection = 20;
						}
						else {
							finalPrefixSelection = 43;
						}
					}
					if (finalPrefixSelection == 42 || finalPrefixSelection == 76) {
						if (!ContentSamples.ItemsByType[finalItemSelection].accessory) {
							finalPrefixSelection = 42;
						}
						else {
							finalPrefixSelection = 76;
						}
					}
				}
				Main.mouseItem.SetDefaults(finalItemSelection);
				Main.mouseItem.stack = stack;
				Main.mouseItem.prefix = (byte)finalPrefixSelection;
				Main.mouseItem.Refresh();
				string text5 = "";
				if (Main.mouseItem.prefix > 0) {
					text5 = " " + MystagogueCommand.Prefixes[(int)Main.mouseItem.prefix];
				}
				Main.NewText(
					"Set cursor item to " + Main.mouseItem.stack +
					(Main.mouseItem.prefix > 0 ? (" " + MystagogueCommand.Prefixes[(int)Main.mouseItem.prefix]) : "") + " " +
					Lang.GetItemNameValue(Main.mouseItem.type) + " (" + Main.mouseItem.type + ")", 255, 255, 255);
			});
		}

		public string commandName;

		public string commandDescription;

		public Action<List<string>> commandAction;

		public static List<MystagogueCommand> commandList = new List<MystagogueCommand>();


		private static string[] Prefixes = new string[]
		{
			"Basic",
			"Large",
			"Massive",
			"Dangerous",
			"Savage",
			"Sharp",
			"Pointy",
			"Tiny",
			"Terrible",
			"Small",
			"Dull",
			"Unhappy",
			"Bulky",
			"Shameful",
			"Heavy",
			"Light",
			"Sighted",
			"Rapid",
			"Hasty",
			"Intimidating",
			"Deadly",
			"Staunch",
			"Awful",
			"Lethargic",
			"Awkward",
			"Powerful",
			"Mystic",
			"Adept",
			"Masterful",
			"Inept",
			"Ignorant",
			"Deranged",
			"Intense",
			"Taboo",
			"Celestial",
			"Furious",
			"Keen",
			"Superior",
			"Forceful",
			"Broken",
			"Damaged",
			"Shoddy",
			"Quick",
			"Deadly",
			"Agile",
			"Nimble",
			"Murderous",
			"Slow",
			"Sluggish",
			"Lazy",
			"Annoying",
			"Nasty",
			"Manic",
			"Hurtful",
			"Strong",
			"Unpleasant",
			"Weak",
			"Ruthless",
			"Frenzying",
			"Godly",
			"Demonic",
			"Zealous",
			"Hard",
			"Guarding",
			"Armored",
			"Warding",
			"Arcane",
			"Precise",
			"Lucky",
			"Jagged",
			"Spiked",
			"Angry",
			"Menacing",
			"Brisk",
			"Fleeting",
			"Hasty",
			"Quick",
			"Wild",
			"Rash",
			"Intrepid",
			"Violent",
			"Legendary",
			"Unreal",
			"Mythical"
		};
	}
}