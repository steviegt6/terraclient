using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.Terraclient.Implementation.Defaults;
using Terraria.Terraclient.Loading;

namespace Terraria.Terraclient.Implementation.Systems
{
	[TerraclientAssociated]
	public class GodModeSystem : ModPlayer
	{
		public override void PreUpdate() {
			base.PreUpdate();

			if (ModContent.GetCheatWithFallback<GodModeCheat>().IsEnabled)
				ResetStats();
		}

		public override void PostUpdate() {
			base.PostUpdate();

			if (ModContent.GetCheatWithFallback<GodModeCheat>().IsEnabled)
				ResetStats();
		}

		public void ResetStats() {
			Player.statLife = Player.statLifeMax2;
			Player.statMana = Player.statManaMax2;
			Player.breath = Player.breathMax;
			Player.breathCD = Player.breathCDMax;
		}

		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore,
			ref PlayerDeathReason damageSource) => !ModContent.GetCheatWithFallback<GodModeCheat>().IsEnabled;
	}
}