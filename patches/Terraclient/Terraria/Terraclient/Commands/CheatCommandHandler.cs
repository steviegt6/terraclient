namespace Terraria.Terraclient.Commands
{
	public static class CheatCommandHandler
	{
		public static bool ParseCheatCommand(string message) {
			if (!message.StartsWith("."))
				return false;

			return true;
		}
	}
}