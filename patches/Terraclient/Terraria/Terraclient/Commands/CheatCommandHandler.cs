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
			catch {
				CheatCommandUtils.Output(true, "Something went wrong.");
				CheatCommandUtils.Output(false, $"Registered text: {string.Join(" ", arguments)}");
			}

			return true;
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