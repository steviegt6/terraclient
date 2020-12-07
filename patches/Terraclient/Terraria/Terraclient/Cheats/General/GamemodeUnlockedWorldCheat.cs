using Terraria.Localization;

namespace Terraria.Terraclient.Cheats.General
{
	public class GamemodeUnlockedWorldCheat : ICheat
	{
		public bool isEnabled = false;

		public string Name() => Language.GetTextValue("Cheats.GameModeUnlockedWorldName");

		public string Description() => Language.GetTextValue("Cheats.GameModeUnlockedWorldCheat");

		public bool IsEnabled() => isEnabled;

		public void SwitchEnabled() => isEnabled = !isEnabled;
	}
}
