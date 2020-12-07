using System.IO;
using Terraria.IO;
using Terraria.Terraclient.Cheats.General;

namespace Terraria.Terraclient
{
	public static class CheatHandler
	{
		public static string CheatPath = Main.SavePath + Path.DirectorySeparatorChar + "Terraclient";

		public static bool IsInATShockServer = false;

		public static GodModeCheat GodMode = new GodModeCheat();

		public static MapTeleportCheat MapTeleport = new MapTeleportCheat();

		public static JourneyModeCheat JourneyMode = new JourneyModeCheat();

		public static GamemodeUnlockedWorldCheat GamemodeUnlockedWorld = new GamemodeUnlockedWorldCheat();

		public static Preferences CheatConfiguration = new Preferences(CheatPath + Path.DirectorySeparatorChar + "cheat_config.json");

		public static void SaveCheatSettings() {
			CheatConfiguration.Clear();

			CheatConfiguration.Put("GodMode", GodMode.isEnabled);
			CheatConfiguration.Put("MapTeleport", MapTeleport.isEnabled);
			CheatConfiguration.Put("JourneyMode", JourneyMode.isEnabled);
			CheatConfiguration.Put("GamemodeUnlockedWorld", GamemodeUnlockedWorld.isEnabled);

			CheatConfiguration.Save();
		}

		public static void LoadCheatSettings() {
			CheatConfiguration.Load();

			CheatConfiguration.Get("GodMode", ref GodMode.isEnabled);
			CheatConfiguration.Get("MapTeleport", ref MapTeleport.isEnabled);
			CheatConfiguration.Get("JourneyMode", ref JourneyMode.isEnabled);
			CheatConfiguration.Get("GamemodeUnlockedWorld", ref GamemodeUnlockedWorld.isEnabled);
		}
	}
}
