using Terraria.Localization;

namespace Terraria.Terraclient.Cheats.General
{
	public class GamemodeUnlockedWorldCheat : Cheat
	{
		public override string Name => Language.GetTextValue("Cheats.GamemodeUnlockedWorldsName");

		public override string Description => Language.GetTextValue("Cheats.GamemodeUnlockedWorldsDesc");
	}
}
