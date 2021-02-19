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

		public override bool IsImportant => true; // only marked as important since there're an uneven amount of misc. cheats

		public override CheatCategory Category => CheatCategory.Fullbright;
	}
}
