using Terraria.Localization;

namespace Terraria.Terraclient.Cheats.General
{
	public class GodModeCheat : ICheat
	{
		public bool isEnabled = true;

		public string Name() => Language.GetTextValue("Cheats.GodModeName");

		public string Description() => Language.GetTextValue("Cheats.GodModeDesc");

		public bool IsEnabled() => isEnabled;

        public void SwitchEnabled() => isEnabled = !isEnabled;
	}
}
