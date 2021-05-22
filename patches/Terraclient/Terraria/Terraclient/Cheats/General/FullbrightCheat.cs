namespace Terraria.Terraclient.Cheats.General
{
	public class FullbrightCheat : FBCheat
	{
		public class FullbrightDimOptionCheat : FBCheat
		{
			public override CheatCategory Category => CheatCategory.Fullbright;
		}

		public class FullbrightLightQualityOptionCheat : FBCheat
		{
			public override CheatCategory Category => CheatCategory.Fullbright;
		}

		public override bool IsImportant => true;

		public override CheatCategory Category => CheatCategory.Fullbright;
	}
}