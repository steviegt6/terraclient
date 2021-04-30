using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Terraria.Terraclient.Commands
{
	public static class CheatCommandHandler
	{
		private static int colorTimer;

		public static bool ParseCheatCommand(string message) {
			if (!message.StartsWith(".") || message.Length == 1)
				return false;

			List<string> arguments = SplitUpMessage(message);
			string query = arguments[0][1..].ToLower();
			arguments.RemoveAt(0);

			try {
				for (int i = 0; i < MystagogueCommand.commandList.Count; i++) {
					if (MystagogueCommand.commandList.ElementAt(i).commandName.ToLower() != query)
						continue;

					MystagogueCommand.commandList[i].commandAction(arguments);
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
			colorTimer++;

			if (colorTimer != 5)
				return;

			List<string> errorColors = CheatCommandUtils.ErrorColors;
			List<string> safeColors = CheatCommandUtils.NonErrorColors;
			string errorColor = errorColors[^2];
			string safeColor = safeColors[^2];

			errorColors.RemoveAt(errorColors.Count - 2);
			errorColors.Insert(1, errorColor);
			safeColors.RemoveAt(safeColors.Count - 2);
			safeColors.Insert(1, safeColor);

			colorTimer = 0;
		}

		private static List<string> SplitUpMessage(string message) =>
			new Regex(@"[\""].+?[\""]|[^ ]+").Matches(message).Select(x => x.Value).ToList();
	}
}