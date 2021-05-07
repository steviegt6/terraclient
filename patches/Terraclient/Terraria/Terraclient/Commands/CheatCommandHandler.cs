using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Terraria.Terraclient.Commands
{
	public static class CheatCommandHandler
	{
		private static int _colorTimer;

		public static bool ParseCheatCommand(string message) {
			if (!message.StartsWith(".") || message.Length == 1)
				return false;

			List<string> arguments = SplitUpMessage(message);
			string query = arguments[0][1..].ToLower();
			arguments.RemoveAt(0);

			try {
				for (int i = 0; i < MystagogueCommand.CommandList.Count; i++) {
					if (MystagogueCommand.CommandList.ElementAt(i).CommandName.ToLower() != query)
						continue;

					MystagogueCommand.CommandList[i].CommandActions.ForEach(x => x(arguments));
					break;
				}
			}
			catch (Exception e) {
				CheatCommandUtils.Output(true, "Something went wrong. " + e.Message, 4);
				CheatCommandUtils.Output(false, $"Registered text: {string.Join(" ", arguments)}");
			}

			return true;
		}

		public static List<object> Digest(List<string> arguments, List<CommandArgument> argumentDetails) {
			if (argumentDetails.Count == 0) {
				return CheatCommandUtils.ConvertToObjects(arguments);
			}
			List<object> polished = new List<object>();
			//Quotation concatenation is handled before this preprocessing, effectively making this post-preprocessing.
			for (int i = 0; i < argumentDetails.Count; i++) {
				if (arguments.Count == 0 && !argumentDetails[i].MayBeSkipped) {
					if (polished.Count == 0) {
						CheatCommandUtils.Output(true, "This command requires arguments to run.", 1);
					}
					else {
						List<string> missingArguments = new List<string>();
						for (int j = i; j < argumentDetails.Count; j++) {
							missingArguments.Add(argumentDetails[j].ArgumentName);
						}
						CheatCommandUtils.Output(true, "You did not submit all the required arguments."
							+ " Every required argument will be before the optional arguments, no matter what command you run."
							+ " Please try again. Missing arguments: " + string.Join(", ", missingArguments), 1);
					}
					goto Errored;
				}
				if (arguments.Count == 0)
					break;

				//declaring these here because declaring inside switch statements caused variable names to no longer be available
				List<string> matches = new List<string> { };
				string query = "";
				int indexOfFirstOmitted = arguments.Count;

				switch (argumentDetails[i].InputType) {
					case CommandArgument.PositiveIntegerRange:
						if (new Regex("\\D").IsMatch(arguments[0])) {
							CheatCommandUtils.Output(true, argumentDetails[i].ArgumentName + " must be a positive integer.", 1);
							goto Errored;
						}

					IAmANumber:

						string parsingString = arguments[0];

						while (parsingString.StartsWith("0"))
							parsingString = parsingString.Remove(0, 1);

						polished.Add(0);

						if (parsingString.Length > argumentDetails[i].ExpectedInputs[1].ToString().Length)
							polished[polished.Count - 1] = argumentDetails[i].ExpectedInputs[1];
						else if (argumentDetails[i].ExpectedInputs[1].ToString().Length == 10 && Convert.ToInt64(parsingString) > 2147483647L)
							polished[polished.Count - 1] = argumentDetails[i].ExpectedInputs[1];
						else if (parsingString.Length > 0)
							polished[polished.Count - 1] = int.Parse(parsingString);

						arguments.RemoveAt(0);
						break;

					case CommandArgument.Text:
						query = arguments[0].ToLower();

						for (int j = 0; j < argumentDetails[i].ExpectedInputs.Count; j++)
							if (CheatCommandUtils.ConvertToStrings(argumentDetails[i].ExpectedInputs)[j].ToLower().StartsWith(query))
								matches.Add((string)argumentDetails[i].ExpectedInputs[j]);

						switch (matches.Count) {
							case 0:
								CheatCommandUtils.Output(true, "Input for argument " + argumentDetails[i].ArgumentName + " did not autocomplete to or directly match any options for " + argumentDetails[i].ArgumentName + ".", 3);
								goto Errored;

							case > 1:
								foreach (string thing in matches) {
									if (thing.Equals(query)) {
										polished.Add(thing);
										matches.Remove(thing);
										CheatCommandUtils.Output(false, "Input for argument " + argumentDetails[i].ArgumentName + " found a direct match (" + thing + "), skipping other results including " +
											string.Join(", ", matches));
										arguments.RemoveAt(0);
										goto CanNowMoveToNextArgument;
									}
								}
								CheatCommandUtils.Output(true, "Input for argument " + argumentDetails[i].ArgumentName + " was too unspecific; There was more than one selection. Make your query longer so it more closely matches the desired selection. Results included " +
									string.Join(", ", matches), 2);
								goto Errored;
						}
						polished.Add(matches[0]);
						arguments.RemoveAt(0);
						break;

					case CommandArgument.TextConcatenationUntilNextInt:
						for (int j = 1; j < arguments.Count; j++) {
							if (new Regex("\\D").IsMatch(arguments[j]))
								continue;
							indexOfFirstOmitted = j;
							break;
						}
						query = string.Join(" ", arguments.GetRange(0, indexOfFirstOmitted)).ToLower();

						for (int j = 0; j < argumentDetails[i].ExpectedInputs.Count; j++)
							if (CheatCommandUtils.ConvertToStrings(argumentDetails[i].ExpectedInputs)[j].ToLower().StartsWith(query))
								matches.Add((string)argumentDetails[i].ExpectedInputs[j]);

						switch (matches.Count) {
							case 0:
								CheatCommandUtils.Output(true, "Input for argument " + argumentDetails[i].ArgumentName + " did not autocomplete to or directly match any options for " + argumentDetails[i].ArgumentName + ".", 3);
								goto Errored;

							case > 1:
								foreach (string thing in matches) {
									if (thing.Equals(query)) {
										polished.Add(thing);
										matches.Remove(thing);
										CheatCommandUtils.Output(false, "Input for argument " + argumentDetails[i].ArgumentName + " found a direct match (" + thing + "), skipping other results including " +
											string.Join(", ", matches));
										arguments.RemoveRange(0, indexOfFirstOmitted);
										goto CanNowMoveToNextArgument;
									}
								}
								CheatCommandUtils.Output(true, "Input for argument " + argumentDetails[i].ArgumentName + " was too unspecific; There was more than one selection. Make your query longer so it more closely matches the desired selection. Results included " +
									string.Join(", ", matches), 2);
								goto Errored;
						}
						polished.Add(matches[0]);
						arguments.RemoveRange(0, indexOfFirstOmitted);
						break;

					case CommandArgument.PositiveIntegerRangeOrTextConcatenationUntilNextInt:
						if (!new Regex("\\D").IsMatch(arguments[0])) {
							goto IAmANumber;
						}
						for (int j = 1; j < arguments.Count; j++) {
							if (new Regex("\\D").IsMatch(arguments[j]))
								continue;
							indexOfFirstOmitted = j;
							break;
						}
						query = string.Join(" ", arguments.GetRange(0, indexOfFirstOmitted)).ToLower();

						for (int j = 0; j < argumentDetails[i].ExpectedInputs.Count; j++)
							if (CheatCommandUtils.ConvertToStrings(argumentDetails[i].ExpectedInputs)[j].ToLower().StartsWith(query))
								matches.Add((string)argumentDetails[i].ExpectedInputs[j]);

						switch (matches.Count) {
							case 0:
								CheatCommandUtils.Output(true, "Input for argument " + argumentDetails[i].ArgumentName + " did not autocomplete to or directly match any options for " + argumentDetails[i].ArgumentName + ".", 3);
								goto Errored;

							case > 1:
								foreach (string thing in matches) {
									if (thing.Equals(query)) {
										polished.Add(thing);
										matches.Remove(thing);
										CheatCommandUtils.Output(false, "Input for argument " + argumentDetails[i].ArgumentName + " found a direct match (" + thing + "), skipping other results including " +
											string.Join(", ", matches));
										arguments.RemoveRange(0, indexOfFirstOmitted);
										goto CanNowMoveToNextArgument;
									}
								}
								CheatCommandUtils.Output(true, "Input for argument " + argumentDetails[i].ArgumentName + " was too unspecific; There was more than one selection. Make your query longer so it more closely matches the desired selection. Results included " +
									string.Join(", ", matches), 2);
								goto Errored;
						}
						polished.Add(matches[0]);
						arguments.RemoveRange(0, indexOfFirstOmitted);
						break;

					case CommandArgument.CustomText:
						polished.Add(arguments[0]);
						arguments.RemoveAt(0);
						break;

					case CommandArgument.CustomTextConcatenationUntilNextInt:
						for (int j = 1; j < arguments.Count; j++) {
							if (new Regex("\\D").IsMatch(arguments[j]))
								continue;
							indexOfFirstOmitted = j;
							break;
						}
						polished.Add(string.Join(" ", arguments.GetRange(0, indexOfFirstOmitted)));
						arguments.RemoveRange(0, indexOfFirstOmitted);
						break;
				}

			CanNowMoveToNextArgument:
				continue;
			}
			return polished;
		Errored:
			return new List<object> { false };
		}

		internal static void UpdateColors() {
			_colorTimer++;

			if (_colorTimer != 5)
				return;

			List<string> errorColors = CheatCommandUtils.ErrorColors;
			List<string> safeColors = CheatCommandUtils.NonErrorColors;
			string errorColor = errorColors[^2];
			string safeColor = safeColors[^2];

			errorColors.RemoveAt(errorColors.Count - 2);
			errorColors.Insert(1, errorColor);
			safeColors.RemoveAt(safeColors.Count - 2);
			safeColors.Insert(1, safeColor);

			_colorTimer = 0;
		}

		public static string GetChatOverlayText() {
			if (Main.chatText is "" or ".") {
				return ".help";
			}

			if (!Main.chatText.StartsWith(".") || Main.chatText.Contains(" ")) {
				return "";
			}

			IDictionary<string, MystagogueCommand> commandsThatStartWithThis = MystagogueCommand.CommandList.Where(
					cmd => $".{cmd.CommandName.ToLower()}"
						.StartsWith(Main.chatText.ToLower()))
				.ToDictionary(cmd => cmd.CommandName);

			if (commandsThatStartWithThis.Count == 0)
				return Main.chatText + " No command found."; // todo: localization

			if (commandsThatStartWithThis.Count == 1)
				return "." + commandsThatStartWithThis.ElementAt(0).Value.CommandName + " " +
				   commandsThatStartWithThis.ElementAt(0).Value.CommandDescription;

			commandsThatStartWithThis = (from pair
						in commandsThatStartWithThis
										 orderby pair.Key
										 select pair)
				.ToDictionary(x => x.Key,
					x => x.Value);

			return "." + string.Join(", ", commandsThatStartWithThis.Keys);
		}

		private static List<string> SplitUpMessage(string message) =>
			new Regex(@"[\""].+?[\""]|[^ ]+").Matches(message).Select(x => x.Value).ToList();
	}
}