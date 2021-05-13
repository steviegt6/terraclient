﻿using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Terraclient.Cheats;
using Terraria.Terraclient.Cheats.General;

namespace Terraria.Terraclient.Commands
{
	public class MystagogueCommand
	{
		private MystagogueCommand(string commandName) {
			CommandName = commandName;
		}

		public static MystagogueCommand Create(string commandName) =>
			new(commandName);

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
			List<object> idsRangeItemNames = new List<object> { 0, ItemID.Count - 1 };
			List<object> idsRangePrefixNames = new List<object> { 0, Prefixes.Length - 1 };
			idsRangeItemNames.AddRange(CheatCommandUtils.ItemNames.Values);
			idsRangePrefixNames.AddRange(Prefixes);

			Create("help")
				.AddParameters(new List<CommandArgument> {
					new("Command Query", CommandListNames.ToList<object>(), false, true)
				})
				.AddAction(args => {
					if (args.Count > 0) {
						MystagogueCommand match = CommandList.FirstOrDefault(t =>
							t.CommandName.StartsWith((string)args[0], StringComparison.OrdinalIgnoreCase));
						string argsText = "";

						if (match == null)
							return;

						foreach (CommandArgument arg in match.CommandArgumentDetails[0]) {
							argsText += (argsText.Length > 0 ? ", " : "") + "(" + arg.ArgumentName +
										": Input type: Input accepts ";
							switch (arg.InputType) {
								case CommandArgument.ArgInputType.PositiveIntegerRange:
									argsText += "a number in between " + arg.ExpectedInputs[0] + " and " +
												arg.ExpectedInputs[1];
									break;

								case CommandArgument.ArgInputType.Text:
									argsText += "specific text options";
									break;

								case CommandArgument.ArgInputType.PositiveIntegerRangeOrText:
									argsText += "specific text options or a number in between " +
												arg.ExpectedInputs[0] + " and " + arg.ExpectedInputs[1];
									break;

								case CommandArgument.ArgInputType.TextConcatenationUntilNextInt:
									argsText += "specific text options";
									break;

								case CommandArgument.ArgInputType
									.PositiveIntegerRangeOrTextConcatenationUntilNextInt:
									argsText += "specific text options or a number in between " +
												arg.ExpectedInputs[0] + " and " + arg.ExpectedInputs[1];
									break;

								case CommandArgument.ArgInputType.CustomText:
								case CommandArgument.ArgInputType.CustomTextConcatenationUntilNextInt:
									argsText += "any text";
									break;

								default:
									throw new ArgumentOutOfRangeException();
							}

							argsText += ")";
						}
						CheatCommandUtils.Output(false,
							Language.GetTextValue("CommandOutputs.help_SpecificResponse",
							match.CommandName,
							match.CommandDescription,
							argsText.Length > 0 ? argsText : "There are none."));
					}
					else {
						CheatCommandUtils.Output(false,
							Language.GetTextValue("CommandOutputs.help_StandardResponse",
							CommandList.Count,
							string.Join(", ", CommandListNames)));
					}
				})
				.Build();

			Create("i")
				.AddParameters(new List<CommandArgument> {
					new("ItemID/Name", idsRangeItemNames, true),
					new("Amount", new List<object> {0, int.MaxValue}, false, true),
					new("Prefix", idsRangePrefixNames, false, true)
				})
				.AddAction(args => {
					int itemType = 0;
					int stack = 1;
					int prefix = 0;
					if (args[0] is string str)
						itemType = CheatCommandUtils.ItemNames.Values.ToList().IndexOf(str);
					else
						itemType = (int)args[0];
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

					HandyFunctions.MoveMouseItemToInventory();
					Main.mouseItem.SetDefaults(itemType);
					Main.mouseItem.stack = stack;
					Main.mouseItem.prefix = (byte)prefix;
					Main.mouseItem.Refresh();

					CheatCommandUtils.Output(false,
						Language.GetTextValue("CommandOutputs.i_ItemSpawnedResponse",
						Main.mouseItem.stack,
						Main.mouseItem.prefix > 0 ? (" " + Prefixes[Main.mouseItem.prefix]) : "",
						Lang.GetItemNameValue(Main.mouseItem.type),
						Main.mouseItem.type));
				})
				.Build();

			Create("searchitems")
				.AddParameters(new List<CommandArgument> { new("Keyword to search for", new List<object>(), true) })
				.AddAction(args => {
					List<string> matches = new List<string>();
					for (int j = 1; j < CheatCommandUtils.ItemNames.Count; j++)
						if (CheatCommandUtils.ItemNames[j]
							.Contains((string)args[0], StringComparison.OrdinalIgnoreCase))
							matches.Add(CheatCommandUtils.ItemNames[j] + " (" + CheatCommandUtils.ItemNames.Values
								.ToList().IndexOf(CheatCommandUtils.ItemNames[j]) + ")");
					if (matches.Count < 1) {
						CheatCommandUtils.Output(false, "No item names match.");
						return;
					}

					if (matches.Count > 1)
						matches.Sort();
					CheatCommandUtils.Output(false, "Found these items: " + string.Join(", ", matches));
				})
				.Build();

			Create("ri")
				.AddParameters(new List<CommandArgument>())
				.AddAction(_ => {
					Main.player[Main.myPlayer].HeldItem.Refresh();
					HandyFunctions.ToolGodBuffMyTools();
					CheatCommandUtils.Output(false, "Selected item refreshed.");
				})
				.Build();

			Create("ris")
				.AddParameters(new List<CommandArgument>())
				.AddAction(_ => {
					Main.player[Main.myPlayer].RefreshItems();
					HandyFunctions.ToolGodBuffMyTools();
					CheatCommandUtils.Output(false, "All inventory items refreshed.");
				})
				.Build();

			Create("invclear")
				.AddParameters(new List<CommandArgument>())
				.AddAction(_ => {
					foreach (Item item in Main.player[Main.myPlayer].inventory) {
						if (!item.favorited) {
							item.SetDefaults();
						}
					}

					foreach (Item item in Main.player[Main.myPlayer].bank4.item) {
						item.SetDefaults();
					}

					HandyFunctions.ToolGodBuffMyTools();

					CheatCommandUtils.Output(false,
						"Cleaned all items not favorited in the inventory and all items in the Void inventory.");
				})
				.Build();

			Create("toolgod")
				.AddParameters(new List<CommandArgument>())
				.AddAction(_ => {
					CheatHandler.GetCheat<ToolGodCheat>().Toggle();
					HandyFunctions.ToolGodBuffMyTools();
					CheatCommandUtils.Output(false, "Tool god toggled " + (CheatHandler.GetCheat<ToolGodCheat>().isEnabled ? "on" : "off") + ".");
				})
				.Build();

			Create("setstacks2b")
				.AddParameters(new List<CommandArgument>())
				.AddAction(_ => {
					foreach (Item item in Main.player[Main.myPlayer].inventory) {
						if (item.IsAir || ContentSamples.ItemsByType[item.type].maxStack == 1)
							continue;
						item.stack = int.MaxValue;
					}
					foreach (Item item in Main.player[Main.myPlayer].miscEquips) {
						if (item.IsAir || ContentSamples.ItemsByType[item.type].maxStack == 1)
							continue;
						item.stack = int.MaxValue;
					}
					foreach (Item item in Main.player[Main.myPlayer].bank.item) {
						if (item.IsAir || ContentSamples.ItemsByType[item.type].maxStack == 1)
							continue;
						item.stack = int.MaxValue;
					}
					foreach (Item item in Main.player[Main.myPlayer].bank2.item) {
						if (item.IsAir || ContentSamples.ItemsByType[item.type].maxStack == 1)
							continue;
						item.stack = int.MaxValue;
					}
					foreach (Item item in Main.player[Main.myPlayer].bank3.item) {
						if (item.IsAir || ContentSamples.ItemsByType[item.type].maxStack == 1)
							continue;
						item.stack = int.MaxValue;
					}
					foreach (Item item in Main.player[Main.myPlayer].bank4.item) {
						if (item.IsAir || ContentSamples.ItemsByType[item.type].maxStack == 1)
							continue;
						item.stack = int.MaxValue;
					}
					CheatCommandUtils.Output(false, "Stacks overflown.");
				})
				.Build();

			Create("setstackslegit")
				.AddParameters(new List<CommandArgument>())
				.AddAction(_ => {
					foreach (Item item in Main.player[Main.myPlayer].inventory) {
						if (item.IsAir || ContentSamples.ItemsByType[item.type].maxStack == 1)
							continue;
						item.stack = item.stack > ContentSamples.ItemsByType[item.type].maxStack ? ContentSamples.ItemsByType[item.type].maxStack : item.stack;
					}
					foreach (Item item in Main.player[Main.myPlayer].armor) {
						if (item.IsAir || ContentSamples.ItemsByType[item.type].maxStack == 1)
							continue;
						item.stack = item.stack > ContentSamples.ItemsByType[item.type].maxStack ? ContentSamples.ItemsByType[item.type].maxStack : item.stack;
					}
					foreach (Item item in Main.player[Main.myPlayer].dye) {
						if (item.IsAir || ContentSamples.ItemsByType[item.type].maxStack == 1)
							continue;
						item.stack = item.stack > ContentSamples.ItemsByType[item.type].maxStack ? ContentSamples.ItemsByType[item.type].maxStack : item.stack;
					}
					foreach (Item item in Main.player[Main.myPlayer].miscEquips) {
						if (item.IsAir || ContentSamples.ItemsByType[item.type].maxStack == 1)
							continue;
						item.stack = item.stack > ContentSamples.ItemsByType[item.type].maxStack ? ContentSamples.ItemsByType[item.type].maxStack : item.stack;
					}
					foreach (Item item in Main.player[Main.myPlayer].miscDyes) {
						if (item.IsAir || ContentSamples.ItemsByType[item.type].maxStack == 1)
							continue;
						item.stack = item.stack > ContentSamples.ItemsByType[item.type].maxStack ? ContentSamples.ItemsByType[item.type].maxStack : item.stack;
					}
					foreach (Item item in Main.player[Main.myPlayer].bank.item) {
						if (item.IsAir || ContentSamples.ItemsByType[item.type].maxStack == 1)
							continue;
						item.stack = item.stack > ContentSamples.ItemsByType[item.type].maxStack ? ContentSamples.ItemsByType[item.type].maxStack : item.stack;
					}
					foreach (Item item in Main.player[Main.myPlayer].bank2.item) {
						if (item.IsAir || ContentSamples.ItemsByType[item.type].maxStack == 1)
							continue;
						item.stack = item.stack > ContentSamples.ItemsByType[item.type].maxStack ? ContentSamples.ItemsByType[item.type].maxStack : item.stack;
					}
					foreach (Item item in Main.player[Main.myPlayer].bank3.item) {
						if (item.IsAir || ContentSamples.ItemsByType[item.type].maxStack == 1)
							continue;
						item.stack = item.stack > ContentSamples.ItemsByType[item.type].maxStack ? ContentSamples.ItemsByType[item.type].maxStack : item.stack;
					}
					foreach (Item item in Main.player[Main.myPlayer].bank4.item) {
						if (item.IsAir || ContentSamples.ItemsByType[item.type].maxStack == 1)
							continue;
						item.stack = item.stack > ContentSamples.ItemsByType[item.type].maxStack ? ContentSamples.ItemsByType[item.type].maxStack : item.stack;
					}
					CheatCommandUtils.Output(false, "Stacks regulated.");
				})
				.Build();

			Create("setmaxstacks2b")
				.AddParameters(new List<CommandArgument>())
				.AddAction(_ => {
					foreach (Item item in Main.player[Main.myPlayer].inventory) {
						if (item.IsAir)
							continue;
						item.maxStack = int.MaxValue;
					}
					foreach (Item item in Main.player[Main.myPlayer].miscEquips) {
						if (item.IsAir)
							continue;
						item.maxStack = int.MaxValue;
					}
					foreach (Item item in Main.player[Main.myPlayer].bank4.item) {
						if (item.IsAir)
							continue;
						item.maxStack = int.MaxValue;
					}
					CheatCommandUtils.Output(false, "Max stacks expanded.");
				})
				.Build();

			Create("setmaxstackslegit")
				.AddParameters(new List<CommandArgument>())
				.AddAction(_ => {
					foreach (Item item in Main.player[Main.myPlayer].inventory) {
						if (item.IsAir)
							continue;
						item.maxStack = ContentSamples.ItemsByType[item.type].maxStack;
					}
					foreach (Item item in Main.player[Main.myPlayer].miscEquips) {
						if (item.IsAir)
							continue;
						item.maxStack = ContentSamples.ItemsByType[item.type].maxStack;
					}
					foreach (Item item in Main.player[Main.myPlayer].bank4.item) {
						if (item.IsAir)
							continue;
						item.maxStack = ContentSamples.ItemsByType[item.type].maxStack;
					}
					CheatCommandUtils.Output(false, "Max stacks regulated.");
				})
				.Build();

			Create("favoriteall")
				.AddParameters(new List<CommandArgument>())
				.AddAction(_ => {
					foreach (Item item in Main.player[Main.myPlayer].inventory) {
						if (item.IsAir)
							continue;
						item.favorited = true;
					}
					foreach (Item item in Main.player[Main.myPlayer].miscEquips) {
						if (item.IsAir)
							continue;
						item.favorited = true;
					}
					CheatCommandUtils.Output(false, "All items favorited.");
				})
				.Build();

			Create("refills")
				.AddParameters(new List<CommandArgument>())
				.AddAction(_ => {
					CheatHandler.GetCheat<RefillsCheat>().Toggle();
					CheatCommandUtils.Output(false, "Favorited item refills toggled " + (CheatHandler.GetCheat<RefillsCheat>().isEnabled ? "on" : "off") + ".");
				})
				.Build();

			Create("torch")
				.AddParameters(new List<CommandArgument>())
				.AddAction(_ => {
					HandyFunctions.MoveMouseItemToInventory();
					Main.mouseItem.SetDefaults(ItemID.Torch);
					Main.mouseItem.stack = 1;
					Main.mouseItem.Refresh();
					Main.mouseItem.placeStyle = 99;
					Main.mouseItem.SetNameOverride("torch of loneliness");
				})
				.Build();

			/*Create("unbreakable")
				.AddParameters(new List<CommandArgument>())
				.AddAction(_ => {
					Main.mouseItem.SetDefaults(ItemID.DirtBlock);
					Main.mouseItem.stack = 999;
					Main.mouseItem.Refresh();
					Main.mouseItem.placeStyle = 99;
					Main.mouseItem.SetNameOverride("dirt but it's really stupid");
				})
				.Build();*/

			CommandList[0].CommandArgumentDetails[0][0].ExpectedInputs = CommandListNames.ToList<object>();
		}

		public string CommandName;

		public string CommandDescription {
			get => Language.GetTextValue($"CommandDescriptions.{CommandName}");
			set { }
		}

		public List<Action<List<object>>> CommandActions = new();

		public List<List<CommandArgument>> CommandArgumentDetails = new();

		public static List<MystagogueCommand> CommandList = new();

		private static List<string> _commandListNames = new();

		public static List<string> CommandListNames {
			get {
				if (_commandListNames.Count != 0)
					return _commandListNames;

				foreach (MystagogueCommand cmd in CommandList) {
					_commandListNames.Add(cmd.CommandName);
				}

				return _commandListNames;
			}
			internal set => _commandListNames = value;
		}

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