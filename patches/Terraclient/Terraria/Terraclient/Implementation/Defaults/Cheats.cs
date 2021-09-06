using Terraria.ModLoader;
using Terraria.Terraclient.Cheats;
using Terraria.Terraclient.Loading;

namespace Terraria.Terraclient.Implementation.Defaults
{
	[TerraclientAssociated]
	public abstract class BaseGeneralCheat : ModCheat
	{
		public override ModCheatCategory Category => ModContent.GetInstance<GeneralCategory>();
	}

	[TerraclientAssociated]
	public abstract class BaseGodModeCheat : ModCheat
	{
		public override ModCheatCategory Category => ModContent.GetInstance<GodModeCategory>();
	}

	[TerraclientAssociated]
	public abstract class BaseFullBrightCheat : ModCheat
	{
		public override ModCheatCategory Category => ModContent.GetInstance<FullBrightCategory>();
	}

	[TerraclientAssociated]
	public abstract class BasePlayerESPCheat : ModCheat
	{
		public override ModCheatCategory Category => ModContent.GetInstance<PlayerESPCategory>();
	}

	[TerraclientAssociated]
	public abstract class BaseForceUnlocksCheat : ModCheat
	{
		public override ModCheatCategory Category => ModContent.GetInstance<ForceUnlocksCategory>();
	}

	[TerraclientAssociated]
	public class ToolGodCheat : BaseGeneralCheat
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("Tool God");
			Description.SetDefault("Buff tools to stupid amounts.");
		}
	}

	[TerraclientAssociated]
	public class RefillsCheat : BaseGeneralCheat
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("Refills");
			Description.SetDefault("Instant refills or something. I dunno, lol.");
		}
	}

	[TerraclientAssociated]
	public class PlayerESPCheat : BasePlayerESPCheat
	{
		public override bool LargeBar { get; set; } = true;

		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("Player ESP");
			Description.SetDefault("See players regardless of position + also see their stats.");
		}
	}

	[TerraclientAssociated]
	public class PlayerESPTracerOptionCheat : BasePlayerESPCheat
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("Draw Tracers to Players");
			Description.SetDefault("Player ESP must be enabled for this.");
		}
	}

	[TerraclientAssociated]
	public class PlayerESPExtraInfoOptionCheat : BasePlayerESPCheat
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("Extra Player ESP Info");
			Description.SetDefault("Displays extra information. Player ESP must be enabled for this.");
		}
	}

	[TerraclientAssociated]
	public class NoMainMenuUIScalingCheat : BaseGeneralCheat
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("No Main Menu UI Scaling");
			Description.SetDefault("Disables the gross forced scaling introduced in 1.4.");
		}
	}

	[TerraclientAssociated]
	public class MapTeleportCheat : BaseGeneralCheat
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("Map Teleportation");
			Description.SetDefault("Right-Click on the Fullscreen Map to teleport.");
		}
	}

	[TerraclientAssociated]
	public class JourneySacrificesCheat : BaseForceUnlocksCheat
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("All Journey Sacrifices");
			Description.SetDefault("Unlocks all items in the journey mode creative menu.");
		}
	}

	[TerraclientAssociated]
	public class JourneyModeCheat : BaseGeneralCheat
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("Conditionless Journey Mode");
			Description.SetDefault("Gives you access to journey mode features without journey mode.");
		}
	}

	[TerraclientAssociated]
	public class InfiniteChestReachCheat : BaseGeneralCheat
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("Infinite Chest Reach");
			Description.SetDefault("Access chests freely.");
		}
	}

	[TerraclientAssociated]
	public class GodModeCheat : BaseGodModeCheat
	{
		public override bool LargeBar { get; set; } = true;

		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("God Mode");
			Description.SetDefault("Makes you invincible.");
		}
	}

	[TerraclientAssociated]
	public class GameModeUnlockedWorldCheat : BaseGeneralCheat
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("No Forced Game Modes");
			Description.SetDefault("Join your worlds regardless of whatever game mode your character is.");
		}
	}

	[TerraclientAssociated]
	public class FullBrightCheat : BaseFullBrightCheat
	{
		public override bool LargeBar { get; set; } = true;

		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("Full-Bright");
			Description.SetDefault("Lights up the entire screen.");
		}
	}

	[TerraclientAssociated]
	public class FullBrightLightQualityOptionCheat : BaseFullBrightCheat
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("Lower Light Quality");
			Description.SetDefault("Spaces out lighting to improve performance.");
		}
	}

	[TerraclientAssociated]
	public class FullBrightDimOptionCheat : BaseFullBrightCheat
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("Dimmer Light");
			Description.SetDefault("Dims the light if you don't need it to be too bright.");
		}
	}

	[TerraclientAssociated]
	public class BestiaryForceUnlockCheat : BaseForceUnlocksCheat
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("Unlock Bestiary Entries");
			Description.SetDefault("Fully unlocks all bestiary entries.");
		}
	}

	[TerraclientAssociated]
	public class AltRightClickDuplicationCheat : BaseGeneralCheat
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			DisplayName.SetDefault("Alt + Right-Click Duplication");
			Description.SetDefault("Duplicate items with Alt + Right-Click.");
		}
	}
}