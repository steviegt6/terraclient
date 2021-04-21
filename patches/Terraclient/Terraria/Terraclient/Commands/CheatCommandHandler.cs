using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Terraria.Terraclient.Commands
{
	public static class CheatCommandHandler
	{
		public static bool ParseCheatCommand(string message) {
			if (!message.StartsWith("."))
				return false;
			string[] arguments = SplitUpMessage(message);

			try {
				switch (arguments[0]) {
					case ".help":
						Main.NewText("test");
						break;
				}
			}
			catch (IndexOutOfRangeException) {
				Main.NewText("Invalid argument count.", 255, 0, 0);
				Main.NewText($"Registered text: {string.Join(" ", arguments)}", Color.Gray.R, Color.Gray.G, Color.Gray.B);
			}

			return true;
		}

		private static string[] SplitUpMessage(string message) {
			Regex regex = new Regex(@"[\""].+?[\""]|[^ ]+");

			return regex.Matches(message).Cast<Match>().Select(x => x.Value).ToArray();
		}
	}
}