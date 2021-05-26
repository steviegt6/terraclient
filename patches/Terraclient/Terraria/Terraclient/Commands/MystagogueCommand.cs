using System;
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

		static MystagogueCommand() {
			List<object> idsRangeItemNames = new List<object> { 0, ItemID.Count - 1 };
			List<object> idsRangePrefixNames = new List<object> { 0, Prefixes.Length - 1 };
			idsRangeItemNames.AddRange(CheatCommandUtils.ItemNames.Values);
			idsRangePrefixNames.AddRange(Prefixes);

			Create("help")
				.AddParameters(new List<CommandArgument>()) // adds args later
				.AddAction(args => {
					if (args.Count > 0) {
						MystagogueCommand match = CommandList.FirstOrDefault(t =>
							t.CommandName.Equals((string)args[0]));
						string argsText = "";
						foreach (CommandArgument arg in match.CommandArgumentDetails[0]) {
							argsText += (argsText.Length > 0 ? ", " : "") + arg.ArgumentName + " ";
							argsText += arg.InputType switch {
								CommandArgument.ArgInputType.PositiveIntegerRange => Language.GetTextValue(
									"InputDescriptions.PositiveIntRange", arg.ExpectedInputs[0], arg.ExpectedInputs[1]),
								CommandArgument.ArgInputType.Text => Language.GetTextValue("InputDescriptions.Text"),
								CommandArgument.ArgInputType.TextConcatenationUntilNextInt => Language.GetTextValue(
									"InputDescriptions.Text"),
								CommandArgument.ArgInputType.PositiveIntegerRangeOrText => Language.GetTextValue(
									"InputDescriptions.PositiveIntRangeOrText", arg.ExpectedInputs[0],
									arg.ExpectedInputs[1]),
								CommandArgument.ArgInputType.PositiveIntegerRangeOrTextConcatenationUntilNextInt =>
									Language.GetTextValue("InputDescriptions.PositiveIntRangeOrText",
										arg.ExpectedInputs[0], arg.ExpectedInputs[1]),
								CommandArgument.ArgInputType.CustomText => Language.GetTextValue(
									"InputDescriptions.CustomText"),
								CommandArgument.ArgInputType.CustomTextConcatenationUntilNextInt => Language
									.GetTextValue("InputDescriptions.CustomText"),
								_ => throw new ArgumentOutOfRangeException()
							};
						}

						CheatCommandUtils.Output(false,
							Language.GetTextValue("CommandOutputs.help_Sel",
								match.CommandName,
								match.CommandDescription,
								argsText.Length > 0
									? argsText
									: Language.GetTextValue("CommandOutputs.help_Sel_NoArgs")));
					}
					else {
						CheatCommandUtils.Output(false,
							Language.GetTextValue("CommandOutputs.help_NoSel",
								CommandList.Count,
								string.Join(", ", CommandListNames),
								CheatHandler.Version));
					}
				})
				.Build();

			Create("i")
				.AddParameters(new List<CommandArgument> {
					new(Language.GetTextValue("CommandArguments.i_ItemNameOrID"), idsRangeItemNames, true),
					new(Language.GetTextValue("CommandArguments.i_ItemStack"), new List<object> {0, int.MaxValue}, false
						, true),
					new(Language.GetTextValue("CommandArguments.i_ItemPrefix"), idsRangePrefixNames, false, true)
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
						if (prefix is 18 or 75)
							prefix = !ContentSamples.ItemsByType[itemType].accessory ? 18 : 75;
						if (prefix is 20 or 43)
							prefix = ContentSamples.ItemsByType[itemType].ranged ? 20 : 43;
						if (prefix is 42 or 76)
							prefix = !ContentSamples.ItemsByType[itemType].accessory ? 42 : 76;
					}

					CheatUtils.MoveMouseItemToInventory();
					Main.mouseItem.SetDefaults(itemType);
					Main.mouseItem.stack = stack;
					Main.mouseItem.prefix = (byte)prefix;
					Main.mouseItem.Refresh();

					CheatCommandUtils.Output(false,
						Language.GetTextValue("CommandOutputs.i_Succ",
							Main.mouseItem.stack,
							Main.mouseItem.prefix > 0 ? (" " + Prefixes[Main.mouseItem.prefix]) : "",
							Lang.GetItemNameValue(Main.mouseItem.type),
							Main.mouseItem.type));
				})
				.Build();

			Create("searchitems")
				.AddParameters(new List<CommandArgument> {
					new(Language.GetTextValue("CommandArguments.searchitems_KeyWordToSearchFor"), new List<object>(),
						true)
				})
				.AddAction(args => {
					List<string> matches = new List<string>();
					for (int j = 1; j < CheatCommandUtils.ItemNames.Count; j++)
						if (CheatCommandUtils.ItemNames[j]
							.Contains((string)args[0], StringComparison.OrdinalIgnoreCase))
							matches.Add(CheatCommandUtils.ItemNames[j] + " (" + j + ")");
					switch (matches.Count) {
						case < 1:
							CheatCommandUtils.Output(false,
								Language.GetTextValue("CommandOutputs.searchitems_NoItemNameFound"));
							return;

						case > 1:
							matches.Sort();
							break;
					}

					CheatCommandUtils.Output(false,
						Language.GetTextValue("CommandOutputs.searchitems_Succ", string.Join(", ", matches)));
				})
				.Build();

			Create("ri")
				.AddParameters(new List<CommandArgument>())
				.AddAction(_ => {
					Main.mouseItem.Refresh();
					CheatUtils.ResetItemName(Main.mouseItem);
					CheatUtils.ToolGodBuffMyTools();
					CheatCommandUtils.Output(false, Language.GetTextValue("CommandOutputs.ri_Succ"));
				})
				.Build();

			Create("ris")
				.AddParameters(new List<CommandArgument>())
				.AddAction(_ => {
					Main.player[Main.myPlayer].RefreshItems();
					CheatUtils.ResetItemNames();
					CheatUtils.ToolGodBuffMyTools();
					CheatCommandUtils.Output(false, Language.GetTextValue("CommandOutputs.ris_Succ"));
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

					CheatUtils.ToolGodBuffMyTools();

					CheatCommandUtils.Output(false, Language.GetTextValue("CommandOutputs.invclear_Succ"));
				})
				.Build();

			Create("toolgod")
				.AddParameters(new List<CommandArgument>())
				.AddAction(_ => {
					CheatHandler.GetCheat<ToolGodCheat>().Toggle();
					CheatUtils.ToolGodBuffMyTools();
					CheatCommandUtils.ToggleMessage(CheatHandler.GetCheat<ToolGodCheat>());
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

					CheatCommandUtils.Output(false, Language.GetTextValue("CommandOutputs.setstacks2b_Succ"));
				})
				.Build();

			Create("setstackslegit")
				.AddParameters(new List<CommandArgument>())
				.AddAction(_ => {
					foreach (Item item in Main.player[Main.myPlayer].inventory) {
						if (item.IsAir)
							continue;
						item.stack = item.stack > ContentSamples.ItemsByType[item.type].maxStack
							? ContentSamples.ItemsByType[item.type].maxStack
							: item.stack;
					}

					foreach (Item item in Main.player[Main.myPlayer].armor) {
						if (item.IsAir)
							continue;
						item.stack = item.stack > ContentSamples.ItemsByType[item.type].maxStack
							? ContentSamples.ItemsByType[item.type].maxStack
							: item.stack;
					}

					foreach (Item item in Main.player[Main.myPlayer].dye) {
						if (item.IsAir)
							continue;
						item.stack = item.stack > ContentSamples.ItemsByType[item.type].maxStack
							? ContentSamples.ItemsByType[item.type].maxStack
							: item.stack;
					}

					foreach (Item item in Main.player[Main.myPlayer].miscEquips) {
						if (item.IsAir)
							continue;
						item.stack = item.stack > ContentSamples.ItemsByType[item.type].maxStack
							? ContentSamples.ItemsByType[item.type].maxStack
							: item.stack;
					}

					foreach (Item item in Main.player[Main.myPlayer].miscDyes) {
						if (item.IsAir)
							continue;
						item.stack = item.stack > ContentSamples.ItemsByType[item.type].maxStack
							? ContentSamples.ItemsByType[item.type].maxStack
							: item.stack;
					}

					foreach (Item item in Main.player[Main.myPlayer].bank.item) {
						if (item.IsAir)
							continue;
						item.stack = item.stack > ContentSamples.ItemsByType[item.type].maxStack
							? ContentSamples.ItemsByType[item.type].maxStack
							: item.stack;
					}

					foreach (Item item in Main.player[Main.myPlayer].bank2.item) {
						if (item.IsAir)
							continue;
						item.stack = item.stack > ContentSamples.ItemsByType[item.type].maxStack
							? ContentSamples.ItemsByType[item.type].maxStack
							: item.stack;
					}

					foreach (Item item in Main.player[Main.myPlayer].bank3.item) {
						if (item.IsAir)
							continue;
						item.stack = item.stack > ContentSamples.ItemsByType[item.type].maxStack
							? ContentSamples.ItemsByType[item.type].maxStack
							: item.stack;
					}

					foreach (Item item in Main.player[Main.myPlayer].bank4.item) {
						if (item.IsAir)
							continue;
						item.stack = item.stack > ContentSamples.ItemsByType[item.type].maxStack
							? ContentSamples.ItemsByType[item.type].maxStack
							: item.stack;
					}

					CheatCommandUtils.Output(false, Language.GetTextValue("CommandOutputs.setstackslegit_Succ"));
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

					CheatCommandUtils.Output(false, Language.GetTextValue("CommandOutputs.setmaxstacks2b_Succ"));
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

					CheatCommandUtils.Output(false, Language.GetTextValue("CommandOutputs.setmaxstackslegit_Succ"));
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

					CheatCommandUtils.Output(false, Language.GetTextValue("CommandOutputs.favoriteall_Succ"));
				})
				.Build();

			Create("refills")
				.AddParameters(new List<CommandArgument>())
				.AddAction(_ => {
					CheatHandler.GetCheat<RefillsCheat>().Toggle();
					CheatCommandUtils.ToggleMessage(CheatHandler.GetCheat<ToolGodCheat>());
				})
				.Build();

			Create("reforge")
				.AddParameters(new List<CommandArgument> {
					new(Language.GetTextValue("CommandArguments.reforge_DesiredPrefix"), idsRangePrefixNames, false, true)
				})
				.AddAction(args => {
					if (Main.mouseItem.IsAir) {
						CheatCommandUtils.Output(true,
							Language.GetTextValue("CommandErrors.reforge_NoItem"));
						return;
					}
					int prefix = 0;
					if (args.Count >= 1) {
						if (args[0] is string)
							prefix = Array.IndexOf(Prefixes, (string)args[0]);
						else
							prefix = (int)args[0];
						if (prefix is 18 or 75)
							prefix = !ContentSamples.ItemsByType[Main.mouseItem.type].accessory ? 18 : 75;
						if (prefix is 20 or 43)
							prefix = ContentSamples.ItemsByType[Main.mouseItem.type].ranged ? 20 : 43;
						if (prefix is 42 or 76)
							prefix = !ContentSamples.ItemsByType[Main.mouseItem.type].accessory ? 42 : 76;
					}
					Main.mouseItem.prefix = (byte)prefix;
					Main.mouseItem.Refresh();
					CheatCommandUtils.Output(false,
						Language.GetTextValue("CommandOutputs.reforge_Succ",
							Main.mouseItem.stack,
							Main.mouseItem.prefix > 0 ? (" " + Prefixes[Main.mouseItem.prefix]) : "",
							Lang.GetItemNameValue(Main.mouseItem.type),
							Main.mouseItem.type));
				})
				.Build();

			Create("damage")
				.AddParameters(new List<CommandArgument> {
					new(Language.GetTextValue("CommandArguments.damage_DesiredDamage"), new List<object> {0, int.MaxValue}, false, true)
				})
				.AddAction(args => {
					if (args.Count == 0) {
						Item reference = Main.LocalPlayer.HeldItem.Clone();
						reference.Refresh();
						if (Main.LocalPlayer.HeldItem.damage == reference.damage)
							Main.LocalPlayer.HeldItem.damage = 999999999;
						else {
							Main.LocalPlayer.HeldItem.damage = reference.damage;
							CheatCommandUtils.Output(false, Language.GetTextValue("CommandOutputs.damage_Reset", Main.LocalPlayer.HeldItem.damage));
							if (reference.Equals(Main.LocalPlayer.HeldItem))
								CheatUtils.ResetItemName(Main.LocalPlayer.HeldItem);
							return;
						}
					}
					else {
						Main.LocalPlayer.HeldItem.damage = (int)args[0];
					}
					CheatCommandUtils.Output(false, Language.GetTextValue("CommandOutputs.damage_Succ", Main.LocalPlayer.HeldItem.damage));
					CheatUtils.MarkItemAsModified(Main.LocalPlayer.HeldItem);
				})
				.Build();

			Create("crit")
				.AddParameters(new List<CommandArgument> {
					new(Language.GetTextValue("CommandArguments.crit_DesiredCrit"), new List<object> {0, 100}, false, true)
				})
				.AddAction(args => {
					if (args.Count == 0) {
						Item reference = Main.LocalPlayer.HeldItem.Clone();
						reference.Refresh();
						if (Main.LocalPlayer.HeldItem.crit == reference.crit)
							Main.LocalPlayer.HeldItem.crit = 100;
						else {
							Main.LocalPlayer.HeldItem.crit = reference.crit;
							CheatCommandUtils.Output(false, Language.GetTextValue("CommandOutputs.crit_Reset", Main.LocalPlayer.HeldItem.crit));
							if (reference.Equals(Main.LocalPlayer.HeldItem))
								CheatUtils.ResetItemName(Main.LocalPlayer.HeldItem);
							return;
						}
					}
					else {
						Main.LocalPlayer.HeldItem.crit = (int)args[0];
					}
					CheatCommandUtils.Output(false, Language.GetTextValue("CommandOutputs.crit_Succ", Main.LocalPlayer.HeldItem.crit));
					CheatUtils.MarkItemAsModified(Main.LocalPlayer.HeldItem);
				})
				.Build();

			Create("ut")
				.AddParameters(new List<CommandArgument> {
					new(Language.GetTextValue("CommandArguments.ut_DesiredUseTime"), new List<object> {0, 999}, false, true)
				})
				.AddAction(args => {
					if (args.Count == 0) {
						Item reference = Main.LocalPlayer.HeldItem.Clone();
						reference.Refresh();
						if (Main.LocalPlayer.HeldItem.useTime == reference.useTime) {
							Main.LocalPlayer.HeldItem.useTime = 0;
							if (Main.LocalPlayer.HeldItem.type == ItemID.Zenith)
								Main.LocalPlayer.HeldItem.useTime = 1;
						}
						else {
							Main.LocalPlayer.HeldItem.useTime = reference.useTime;
							CheatCommandUtils.Output(false, Language.GetTextValue("CommandOutputs.ut_Reset", Main.LocalPlayer.HeldItem.useTime));
							if (reference.Equals(Main.LocalPlayer.HeldItem))
								CheatUtils.ResetItemName(Main.LocalPlayer.HeldItem);
							return;
						}
					}
					else {
						Main.LocalPlayer.HeldItem.useTime = (int)args[0];
					}
					CheatCommandUtils.Output(false, Language.GetTextValue("CommandOutputs.ut_Succ", Main.LocalPlayer.HeldItem.useTime));
					CheatUtils.MarkItemAsModified(Main.LocalPlayer.HeldItem);
				})
				.Build();

			Create("at")
				.AddParameters(new List<CommandArgument> {
					new(Language.GetTextValue("CommandArguments.at_DesiredAnimationTime"), new List<object> {0, 999}, false, true)
				})
				.AddAction(args => {
					if (args.Count == 0) {
						Item reference = Main.LocalPlayer.HeldItem.Clone();
						reference.Refresh();
						if (Main.LocalPlayer.HeldItem.useAnimation == reference.useAnimation)
							Main.LocalPlayer.HeldItem.useAnimation = 3;
						else {
							Main.LocalPlayer.HeldItem.useAnimation = reference.useAnimation;
							CheatCommandUtils.Output(false, Language.GetTextValue("CommandOutputs.at_Reset", Main.LocalPlayer.HeldItem.useAnimation));
							if (reference.Equals(Main.LocalPlayer.HeldItem))
								CheatUtils.ResetItemName(Main.LocalPlayer.HeldItem);
							return;
						}
					}
					else {
						Main.LocalPlayer.HeldItem.useAnimation = (int)args[0];
					}
					CheatCommandUtils.Output(false, Language.GetTextValue("CommandOutputs.at_Succ", Main.LocalPlayer.HeldItem.useAnimation));
					CheatUtils.MarkItemAsModified(Main.LocalPlayer.HeldItem);
				})
				.Build();

			Create("torch")
				.AddParameters(new List<CommandArgument>())
				.AddAction(_ => {
					CheatUtils.MoveMouseItemToInventory();
					Main.mouseItem.SetDefaults(ItemID.Torch);
					Main.mouseItem.stack = 1;
					Main.mouseItem.Refresh();
					Main.mouseItem.placeStyle = 99;
					Main.mouseItem.SetNameOverride(Language.GetTextValue("CustomNames.ExploitTorch"));
					CheatCommandUtils.Output(false, Language.GetTextValue("CommandOutputs.torch_Succ"));
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

			CommandList[0].CommandArgumentDetails[0] = new List<CommandArgument> {
				new(Language.GetTextValue("CommandArguments.help_CommandQuery"), CommandListNames.ToList<object>(),
					false, true)
			};
		}

		public string CommandName;

		public string CommandDescription => Language.GetTextValue($"CommandDescriptions.{CommandName}");

		// set => CommandName = value;

		public List<Action<List<object>>> CommandActions = new();

		public List<List<CommandArgument>> CommandArgumentDetails = new();

		public static List<MystagogueCommand> CommandList { get; } = new();

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