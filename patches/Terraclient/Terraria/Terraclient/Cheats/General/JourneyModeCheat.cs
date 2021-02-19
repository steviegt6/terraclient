using Terraria.Localization;

namespace Terraria.Terraclient.Cheats.General
{
	public class JourneyModeCheat : Cheat
	{
		public override string Name => Language.GetTextValue("Cheats.JourneyModeName");

		public override string Description => Language.GetTextValue("Cheats.JourneyModeDesc");
	}
}
