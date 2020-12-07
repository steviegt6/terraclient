using Terraria.Localization;

namespace Terraria.Terraclient.Cheats.General
{
	public class GamemodeUnlockedWorldCheat : ICheat
	{
		public bool isEnabled = false;

		public string Name() => Language.GetTextValue("Cheats.GamemodeUnlockedWorldName");

		public string Description() => Language.GetTextValue("Cheats.GamemodeUnlockedWorldCheat");

		public bool IsEnabled() => isEnabled;

		public void SwitchEnabled() => isEnabled = !isEnabled;
	}
}
