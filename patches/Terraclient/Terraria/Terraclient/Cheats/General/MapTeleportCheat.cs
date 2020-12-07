namespace Terraria.Terraclient.Cheats.General
{
	public class MapTeleportCheat : ICheat
	{
		public bool isEnabled = true;

		public string Name() => "Map Teleportation";

		public string Description() => "Allows you to right-click anywhere on a map to teleport.";

		public bool IsEnabled() => isEnabled;

		public void SwitchEnabled() => isEnabled = !isEnabled;
	}
}
