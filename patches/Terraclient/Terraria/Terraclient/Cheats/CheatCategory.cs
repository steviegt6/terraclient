namespace Terraria.Terraclient.Cheats
{
	public enum CheatCategory
	{
		Misc,
		GodMode,
		Fullbright,
		// ReSharper disable once InconsistentNaming
		PlayerESP
	}

	public abstract class GMCheat : Cheat
	{
		public override CheatCategory Category => CheatCategory.GodMode;
	}

	public abstract class FBCheat : Cheat
	{
		public override CheatCategory Category => CheatCategory.Fullbright;
	}

	public abstract class PESPCheat : Cheat
	{
		public override CheatCategory Category => CheatCategory.PlayerESP;
	}
}