using System.IO;
using Terraria.Terraclient.Cheats.General;

namespace Terraria.Terraclient
{
	public static class CheatHandler
	{
		public static string CheatPath = Main.SavePath + Path.DirectorySeparatorChar + "Terraclient";

		public static bool IsInATShockServer = false;

		public static GodModeCheat GodMode = new GodModeCheat();

		public static MapTeleportCheat MapTeleport = new MapTeleportCheat();
	}
}
