namespace Terraria.Terraclient.Cheats.General
{
	public class FullbrightCheat : Cheat
	{
		public class FullbrightDimOptionCheat : Cheat
		{
			public override CheatCategory Category => CheatCategory.Fullbright;
		}

		public class FullbrightLightQualityOptionCheat : Cheat
		{
			public override CheatCategory Category => CheatCategory.Fullbright;
		}

		public override bool IsImportant => true;

		public override CheatCategory Category => CheatCategory.Fullbright;
	}
}
