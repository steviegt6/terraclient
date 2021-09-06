using Terraria.Terraclient.Cheats;
using Terraria.Terraclient.Loading;

namespace Terraria.Terraclient.Implementation.Defaults
{
	[TerraclientAssociated]
	public class GeneralCategory : ModCheatCategory
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("General Cheats");
		}
	}

	[TerraclientAssociated]
	public class GodModeCategory : ModCheatCategory
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("God Mode");
		}
	}

	[TerraclientAssociated]
	public class FullBrightCategory : ModCheatCategory
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("Full-Bright");
		}
	}

	[TerraclientAssociated]
	public class PlayerESPCategory : ModCheatCategory
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("Player ESP");
		}
	}

	[TerraclientAssociated]
	public class ForceUnlocksCategory : ModCheatCategory
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("Forceful Unlocking");
		}
	}
}