using Terraria.Localization;

namespace Terraria.Terraclient.Cheats.General
{
	public class GodModeCheat : Cheat
	{
		public override string Name => Language.GetTextValue("Cheats.GodModeName");

		public override string Description => Language.GetTextValue("Cheats.GodModeDesc");

		public override bool IsImportant => true;

		public override CheatCategory Category => CheatCategory.GodMode;
	}
}
