using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Terraria.ID;

namespace Terraria.Terraclient.Commands
{
	public class MystagogueCommand
	{
		private MystagogueCommand(string commandName, string commandDescription) {
			CommandName = commandName;
			CommandDescription = commandDescription;
		}

		public static MystagogueCommand Create(string commandName, string commandDescription) =>
			new(commandName, commandDescription);

		public MystagogueCommand AddAction(Action<List<string>> action) {
			CommandActions.Add(action);
			return this;
		}

		public MystagogueCommand AddParameters(List<CommandArgument> arguments) {
			CommandArgumentDetails.Add(arguments);
			return this;
		}

		public MystagogueCommand Build() {
			CommandList.Add(this);
			return this;
		}

		// TODO: localization
		static MystagogueCommand() {
			Create("help", "Returns \"test\" in chat")
				.AddAction(_ => CheatCommandUtils.Output(false, "test"))
				.Build();

			Create("torch",
					"Gives the player a torch with an invalid torch ID, which can cause crashes on other clients and the server.")
				.AddAction(_ => {
					Main.mouseItem.SetDefaults(ItemID.Torch);
					Main.mouseItem.stack = 1;
					Main.mouseItem.Refresh();
					Main.mouseItem.placeStyle = 99;
					Main.mouseItem.SetNameOverride("cum torch of cum or something else that's funny");
				})
				.Build();

			Create("unbreakable",
					"Gives the player a stack of unbreakable dirt blocks that, when placed, are not rendered on any client.")
				.AddAction(_ => {
					Main.mouseItem.SetDefaults(ItemID.DirtBlock);
					Main.mouseItem.stack = 999;
					Main.mouseItem.Refresh();
					Main.mouseItem.placeStyle = 99;
					Main.mouseItem.SetNameOverride("block of cock block");
				})
				.Build();

			Create("i",
					"Spawns an item by converting your currently-held cursor item (or thin air) to it. " +
					"Automatically sent to your inventory if your inventory is closed.")
				.AddAction(args => {
					if (args.Count == 0) {
						CheatCommandUtils.Output(true, "That command requires arguments", 1);
						return;
					}

					int itemType = 0;
					if (!new Regex("\\D").IsMatch(args[0])) {
						string text = args[0];

						while (text.StartsWith("0"))
							text = text.Remove(0, 1);

						while (text != itemType.ToString()) {
							if (itemType == ItemID.Count) {
								CheatCommandUtils.Output(true, "Given item ID does not correspond to an item", 3);
								return;
							}

							itemType++;
						}

						args.RemoveAt(0);
					}
					else {
						int stackIndex = 1;

						if (args.Count < 0) {
							while (new Regex("\\D").IsMatch(args[stackIndex])) {
								stackIndex++;

								if (stackIndex == args.Count)
									break;
							}
						}

						string nameQuery = string.Join(" ", args.GetRange(0, stackIndex)).ToLower();
						args = args.GetRange(stackIndex, args.Count - stackIndex);

						List<int> foundItems = new List<int>();

						for (int i = 0; i < ItemID.Count; i++)
							if (Lang.GetItemNameValue(i).ToLower().StartsWith(nameQuery))
								foundItems.Add(i);

						switch (foundItems.Count) {
							case 0:
								CheatCommandUtils.Output(true, "No item names match", 3);
								return;

							case > 1: {
									List<string> itemNameListToCheck =
										foundItems.Select(id => Lang.GetItemNameValue(id).ToLower()).ToList();
									List<string> itemNameAndIDsToOutput = foundItems
										.Select(id => string.Concat(Lang.GetItemNameValue(id), " (", id, ")")).ToList();

									for (int k = 0; k < itemNameListToCheck.Count; k++) {
										if (itemNameListToCheck[k] == nameQuery) {
											itemType = foundItems[k];
											itemNameAndIDsToOutput.RemoveAt(k);
											CheatCommandUtils.Output(false,
												"Other matches include " + string.Join(", ", itemNameAndIDsToOutput));
											break;
										}

										if (k + 1 != foundItems.Count)
											continue;

										CheatCommandUtils.Output(true,
											"Query too unspecific, found " + string.Join(", ", itemNameAndIDsToOutput), 2);
										return;
									}

									break;
								}

							default:
								itemType = foundItems[0];
								break;
						}
					}

					int stack = 1;
					if (args.Count >= 1) {
						if (new Regex("\\D").IsMatch(args[0])) {
							CheatCommandUtils.Output(true, "Stack must be a positive integer", 1);
							return;
						}

						string parsingString = args[0];

						while (parsingString.StartsWith("0"))
							parsingString = parsingString.Remove(0, 1);

						if (parsingString.Length > 10)
							stack = int.MaxValue;
						else if (Convert.ToInt64(parsingString) > 2147483647L)
							stack = int.MaxValue;
						else if (parsingString.Length > 0)
							stack = int.Parse(parsingString);
					}

					int finalPrefixSelection = 0;

					if (args.Count >= 2) {
						if (!new Regex("\\D").IsMatch(args[1])) {
							string parsingString = args[1];

							while (parsingString.StartsWith("0"))
								parsingString = parsingString.Remove(0, 1);

							while (parsingString != finalPrefixSelection.ToString()) {
								if (finalPrefixSelection == PrefixID.Count) {
									CheatCommandUtils.Output(true, "Given prefix ID does not correspond to a prefix", 3);
									return;
								}

								finalPrefixSelection++;
							}
						}
						else {
							string parsingString = args[1].ToLower();
							List<int> foundPrefixes = new List<int>();

							for (int i = 0; i < Prefixes.Length; i++)
								if (Prefixes[i].ToLower().StartsWith(parsingString) && i != 75 && i != 43 && i != 76)
									foundPrefixes.Add(i);

							switch (foundPrefixes.Count) {
								case 0:
									CheatCommandUtils.Output(true, "No prefix names match", 3);
									return;

								case > 1: {
										List<string> prefixNamesAndIDsToOutput = foundPrefixes
											.Select(i => string.Concat(Prefixes[i], " (", i, ")")).ToList();
										CheatCommandUtils.Output(true,
											"Prefix query too unspecific, found " +
											string.Join(", ", prefixNamesAndIDsToOutput), 2);
										return;
									}

								default:
									finalPrefixSelection = foundPrefixes[0];
									break;
							}
						}

						if (finalPrefixSelection is 18 or 75) {
							finalPrefixSelection = !ContentSamples.ItemsByType[itemType].accessory
								? 18
								: 75;
						}

						if (finalPrefixSelection is 20 or 43) {
							finalPrefixSelection = ContentSamples.ItemsByType[itemType].ranged
								? 20
								: 43;
						}

						if (finalPrefixSelection is 42 or 76) {
							finalPrefixSelection = !ContentSamples.ItemsByType[itemType].accessory
								? 42
								: 76;
						}
					}

					Main.mouseItem.SetDefaults(itemType);
					Main.mouseItem.stack = stack;
					Main.mouseItem.prefix = (byte)finalPrefixSelection;
					Main.mouseItem.Refresh();

					CheatCommandUtils.Output(false,
						$"Set cursor item to {Main.mouseItem.stack}{(Main.mouseItem.prefix > 0 ? (" " + Prefixes[Main.mouseItem.prefix]) : "")} {Lang.GetItemNameValue(Main.mouseItem.type)} ({Main.mouseItem.type})");
				})
				.Build();

			Create("duplicatenames",
					"Finds all the duplicate item, npc, and buff names in the game and outputs them to chat.")
				.AddAction(_ => {
					//ITEMS
					Dictionary<string, int> found = new Dictionary<string, int>();
					for (int i = 0; i < ItemID.Count; i++) {
						if (!found.ContainsKey(CheatCommandUtils.ItemNames[i])) {
							found.Add(CheatCommandUtils.ItemNames[i], 1);
						}
						else {
							found[CheatCommandUtils.ItemNames[i]]++;
						}
					}
					for (int i = 0; i < found.Count; i++) {
						if (found.ElementAt(i).Value == 1) {
							found.Remove(found.ElementAt(i).Key);
							i--;
						}
					}
					string output = "Duplicate item names: ";
					for (int i = 0; i < found.Count; i++) {
						string ids = "";
						for (int j = 0; j < ItemID.Count; j++) {
							if (CheatCommandUtils.ItemNames[j] == found.ElementAt(i).Key) {
								ids += j + ", ";
							}
						}
						ids = ids.Substring(0, ids.Length - 2);
						output += found.ElementAt(i).Key + " (" + found.ElementAt(i).Value + " occurences: " + ids + ")" + ", ";
					}
					output = output.Substring(0, output.Length - 2);
					CheatCommandUtils.Output(false, output);
					//NPCS
					found = new Dictionary<string, int>();
					for (int i = 0; i < NPCID.Count; i++) {
						if (!found.ContainsKey(CheatCommandUtils.NPCNames[i])) {
							found.Add(CheatCommandUtils.NPCNames[i], 1);
						}
						else {
							found[CheatCommandUtils.NPCNames[i]]++;
						}
					}
					for (int i = 0; i < found.Count; i++) {
						if (found.ElementAt(i).Value == 1) {
							found.Remove(found.ElementAt(i).Key);
							i--;
						}
					}
					output = "Duplicate NPC names: ";
					for (int i = 0; i < found.Count; i++) {
						string ids = "";
						for (int j = 0; j < NPCID.Count; j++) {
							if (CheatCommandUtils.NPCNames[j] == found.ElementAt(i).Key) {
								ids += j + ", ";
							}
						}
						ids = ids.Substring(0, ids.Length - 2);
						output += found.ElementAt(i).Key + " (" + found.ElementAt(i).Value + " occurences: " + ids + ")" + ", ";
					}
					output = output.Substring(0, output.Length - 2);
					for (int i = 0; true;) {
						if (output.Length < i + 160) {
							CheatCommandUtils.Output(false, output.Substring(i));
							break;
						}
						CheatCommandUtils.Output(false, output.Substring(i, 160));
						i += 160;
					}
					//BUFFS
					found = new Dictionary<string, int>();
					for (int i = 0; i < BuffID.Count; i++) {
						if (!found.ContainsKey(CheatCommandUtils.BuffNames[i])) {
							found.Add(CheatCommandUtils.BuffNames[i], 1);
						}
						else {
							found[CheatCommandUtils.BuffNames[i]]++;
						}
					}
					for (int i = 0; i < found.Count; i++) {
						if (found.ElementAt(i).Value == 1) {
							found.Remove(found.ElementAt(i).Key);
							i--;
						}
					}
					output = "Duplicate buff names: ";
					for (int i = 0; i < found.Count; i++) {
						string ids = "";
						for (int j = 0; j < BuffID.Count; j++) {
							if (CheatCommandUtils.BuffNames[j] == found.ElementAt(i).Key) {
								ids += j + ", ";
							}
						}
						ids = ids.Substring(0, ids.Length - 2);
						output += found.ElementAt(i).Key + " (" + found.ElementAt(i).Value + " occurences: " + ids + ")" + ", ";
					}
					output = output.Substring(0, output.Length - 2);
					CheatCommandUtils.Output(false, output);
				})
				.Build();

		}

		public string CommandName;

		public string CommandDescription;

		public List<Action<List<string>>> CommandActions = new();

		public List<List<CommandArgument>> CommandArgumentDetails = new();

		public static List<MystagogueCommand> CommandList = new();

		private static readonly string[] Prefixes = {
			"Basic", "Large", "Massive", "Dangerous", "Savage", "Sharp", "Pointy", "Tiny", "Terrible", "Small", "Dull",
			"Unhappy", "Bulky", "Shameful", "Heavy", "Light", "Sighted", "Rapid", "Hasty", "Intimidating", "Deadly",
			"Staunch", "Awful", "Lethargic", "Awkward", "Powerful", "Mystic", "Adept", "Masterful", "Inept", "Ignorant",
			"Deranged", "Intense", "Taboo", "Celestial", "Furious", "Keen", "Superior", "Forceful", "Broken", "Damaged",
			"Shoddy", "Quick", "Deadly", "Agile", "Nimble", "Murderous", "Slow", "Sluggish", "Lazy", "Annoying",
			"Nasty", "Manic", "Hurtful", "Strong", "Unpleasant", "Weak", "Ruthless", "Frenzying", "Godly", "Demonic",
			"Zealous", "Hard", "Guarding", "Armored", "Warding", "Arcane", "Precise", "Lucky", "Jagged", "Spiked",
			"Angry", "Menacing", "Brisk", "Fleeting", "Hasty", "Quick", "Wild", "Rash", "Intrepid", "Violent",
			"Legendary", "Unreal", "Mythical"
		};
	}
}