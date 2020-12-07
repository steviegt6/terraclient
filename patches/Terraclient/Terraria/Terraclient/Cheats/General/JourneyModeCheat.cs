using Terraria.Localization;

namespace Terraria.Terraclient.Cheats.General
{
	public class JourneyModeCheat : ICheat
	{
		public bool isEnabled = true;

		public string Name() => Language.GetTextValue("Cheats.JourneyModeName");

		public string Description() => Language.GetTextValue("Cheats.JourneyModeDesc");

		public bool IsEnabled() => isEnabled;

		public void SwitchEnabled() => isEnabled = !isEnabled;
	}
}
