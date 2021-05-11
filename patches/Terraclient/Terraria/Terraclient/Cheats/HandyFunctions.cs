using Terraria.Terraclient.Cheats.General;
using Terraria.UI;
using Terraria.ID;

namespace Terraria.Terraclient.Cheats
{
	public static class HandyFunctions
	{
		public static bool TryDupe(Item slot) {
			if (!CheatHandler.GetCheat<AltRightClickDuplicationCheat>().isEnabled)
				return false;
			if (Main.cursorOverride == 3) {
				if (Main.mouseRight && Main.mouseRightRelease && (Main.mouseItem.IsTheSameAs(slot) | Main.mouseItem.IsAir)) {
					Main.mouseItem = new Item();
					Main.mouseItem.SetDefaults(slot.type);
					Main.mouseItem.Prefix(slot.prefix);
					if (ItemSlot.ControlInUse && Main.mouseItem.maxStack != 1) {
						Main.mouseItem.stack = int.MaxValue;
					}
					else {
						Main.mouseItem.stack = Main.mouseItem.maxStack;
					}
				}
				return true;
			}
			return false;
		}
		public static void ToolGodBuffMyTools() {
			if (!CheatHandler.GetCheat<ToolGodCheat>().isEnabled)
				return;
			int i = 0;
			for (; i < Main.player[Main.myPlayer].inventory.Length; i++) {
				if (Main.player[Main.myPlayer].inventory[i].pick > 0) {
					Main.player[Main.myPlayer].inventory[i].pick = ContentSamples.ItemsByType[2786].pick;
					Main.player[Main.myPlayer].inventory[i].useTime = 0;
					Main.player[Main.myPlayer].inventory[i].useAnimation = 7;
					Main.player[Main.myPlayer].inventory[i].tileBoost = 15;
					break;
				}
			}
			for (i++; i < Main.player[Main.myPlayer].inventory.Length; i++) {
				if (Main.player[Main.myPlayer].inventory[i].pick > 0) {
					Main.player[Main.myPlayer].inventory[i].Refresh();
				}
			}
			for (i = 0; i < Main.player[Main.myPlayer].inventory.Length; i++) {
				if (Main.player[Main.myPlayer].inventory[i].axe > 0) {
					Main.player[Main.myPlayer].inventory[i].axe = ContentSamples.ItemsByType[1305].axe;
					Main.player[Main.myPlayer].inventory[i].useTime = 0;
					Main.player[Main.myPlayer].inventory[i].useAnimation = 7;
					Main.player[Main.myPlayer].inventory[i].tileBoost = 15;
					break;
				}
			}
			for (i++; i < Main.player[Main.myPlayer].inventory.Length; i++) {
				if (Main.player[Main.myPlayer].inventory[i].axe > 0) {
					Main.player[Main.myPlayer].inventory[i].Refresh();
				}
			}
			for (i = 0; i < Main.player[Main.myPlayer].inventory.Length; i++) {
				if (Main.player[Main.myPlayer].inventory[i].hammer > 0) {
					Main.player[Main.myPlayer].inventory[i].hammer = ContentSamples.ItemsByType[1305].hammer;
					Main.player[Main.myPlayer].inventory[i].useTime = 0;
					Main.player[Main.myPlayer].inventory[i].useAnimation = 7;
					Main.player[Main.myPlayer].inventory[i].tileBoost = 4;
					break;
				}
			}
			for (i++; i < Main.player[Main.myPlayer].inventory.Length; i++) {
				if (Main.player[Main.myPlayer].inventory[i].hammer > 0) {
					Main.player[Main.myPlayer].inventory[i].Refresh();
				}
			}
		}
		public static void ResetEffectsMod() {
			if (HeavyTasksTimer > 0)
				HeavyTasksTimer--;
			else if (HeavyTasksTimer == 0) {
				HeavyTasksTimer = 8;
				ToolGodBuffMyTools();
			}
		}
		private static int HeavyTasksTimer;
	}
}
