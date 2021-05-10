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

		public MystagogueCommand AddAction(Action<List<object>> action) {
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
				.AddParameters(new List<CommandArgument>())
				.AddAction(_ => CheatCommandUtils.Output(false, "test"))
				.Build();

			Create("torch",
					"Gives the player a torch with an invalid torch ID, which can cause crashes on other clients and the server.")
				.AddParameters(new List<CommandArgument>())
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
				.AddParameters(new List<CommandArgument>())
				.AddAction(_ => {
					Main.mouseItem.SetDefaults(ItemID.DirtBlock);
					Main.mouseItem.stack = 999;
					Main.mouseItem.Refresh();
					Main.mouseItem.placeStyle = 99;
					Main.mouseItem.SetNameOverride("block of cock block");
				})
				.Build();

			List<object> IDsRangeThenItemNames = new List<object> { 0, ItemID.Count - 1 };
			IDsRangeThenItemNames.AddRange(CheatCommandUtils.ItemNames.Values);
			List<object> IDsRangeThenPrefixNames = new List<object> { 0, Prefixes.Length - 1 };
			IDsRangeThenPrefixNames.AddRange(Prefixes);

			Create("i",
					"Spawns an item by converting your currently-held cursor item (or thin air) to it. " +
					"The item will be automatically sent to your inventory if your inventory is closed.")
				.AddParameters(new List<CommandArgument> {new CommandArgument("ItemID/Name", IDsRangeThenItemNames, true),
					new CommandArgument("Amount", new List<object>{0,int.MaxValue}, false, true),
					new CommandArgument("Prefix", IDsRangeThenPrefixNames, false, true) })
				.AddAction(args => {
					int itemType = 0;
					int stack = 1;
					int prefix = 0;
					if (args[0] is string)
						itemType = CheatCommandUtils.ItemNames.Values.ToList().IndexOf((string)args[0]);
					else
						prefix = (int)args[0];
					if (args.Count >= 2)
						stack = (int)args[1];
					if (args.Count >= 3) {
						if (args[2] is string)
							prefix = Array.IndexOf(Prefixes, (string)args[2]);
						else
							prefix = (int)args[2];
						if (prefix is 18 or 75) {
							prefix = !ContentSamples.ItemsByType[itemType].accessory
								? 18
								: 75;
						}
						if (prefix is 20 or 43) {
							prefix = ContentSamples.ItemsByType[itemType].ranged
								? 20
								: 43;
						}
						if (prefix is 42 or 76) {
							prefix = !ContentSamples.ItemsByType[itemType].accessory
								? 42
								: 76;
						}
					}

					Main.mouseItem.SetDefaults(itemType);
					Main.mouseItem.stack = stack;
					Main.mouseItem.prefix = (byte)prefix;
					Main.mouseItem.Refresh();

					CheatCommandUtils.Output(false,
						$"Set cursor item to {Main.mouseItem.stack}{(Main.mouseItem.prefix > 0 ? (" " + Prefixes[Main.mouseItem.prefix]) : "")} {Lang.GetItemNameValue(Main.mouseItem.type)} ({Main.mouseItem.type})");
				})
				.Build();

			Create("duplicatenames",
					"Finds all the duplicate item, npc, and buff names in the game and outputs them to chat.")
				.AddParameters(new List<CommandArgument>())
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

		public List<Action<List<object>>> CommandActions = new();

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