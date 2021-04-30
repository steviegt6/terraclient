using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Terraria.Terraclient.Commands
{
	public static class CheatCommandHandler
	{
		public static bool ParseCheatCommand(string message) {
			if (!message.StartsWith(".") || message.Length == 1)
				return false;
			List<string> arguments = SplitUpMessage(message);
			string query = arguments[0].Substring(1).ToLower();
			arguments.RemoveAt(0);

			try {
				for (int i = 0; i < MystagogueCommand.commandList.Count; i++) {
					if (MystagogueCommand.commandList.ElementAt(i).commandName.ToLower() == query) {
						MystagogueCommand.commandList[i].commandAction(arguments);
						break;
					}
				}
			}
			catch {
				SpecialChatBehaviorHelpers.Output(true, "Something went wrong.");
				SpecialChatBehaviorHelpers.Output(false, $"Registered text: {string.Join(" ", arguments)}");
			}

			return true;
		}

		private static List<string> SplitUpMessage(string message) {
			Regex regex = new Regex(@"[\""].+?[\""]|[^ ]+");

			return regex.Matches(message).Cast<Match>().Select(x => x.Value).ToList();
		}
	}
}