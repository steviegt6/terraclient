namespace Terraria.Terraclient.Cheats.General
{
	public class GodModeCheat : ICheat
	{
		public bool isEnabled = true;

		public string Name() => "God Mode";

		public string Description() => "Prevents your player from taking damage, suffering from the effects of debuffs, etc.";

		public bool IsEnabled() => isEnabled;

        public void SwitchEnabled() => isEnabled = !isEnabled;
	}
}
