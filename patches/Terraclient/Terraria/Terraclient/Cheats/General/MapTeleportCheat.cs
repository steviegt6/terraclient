using Terraria.Localization;

namespace Terraria.Terraclient.Cheats.General
{
	public class MapTeleportCheat : Cheat
	{
		public override string Name => Language.GetTextValue("Cheats.MapTeleportName");

		public override string Description => Language.GetTextValue("Cheats.MapTeleportDesc");

		public override bool IsImportant => true; // only marked as important since there're an uneven amount of misc. cheats
	}
}
