using Terraria.Localization;

namespace Terraria.Terraclient.Cheats.General
{
	public class GamemodeUnlockedWorldCheat : ICheat
	{
		public bool isEnabled = false;

		public string Name() => Language.GetTextValue("Cheats.GamemodeUnlockedWorldsName");

		public string Description() => Language.GetTextValue("Cheats.GamemodeUnlockedWorldsDesc");

		public bool IsEnabled() => isEnabled;

		public void SwitchEnabled() => isEnabled = !isEnabled;
	}
}
