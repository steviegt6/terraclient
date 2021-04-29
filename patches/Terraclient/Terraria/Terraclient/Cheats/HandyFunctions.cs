using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Terraclient.Cheats.General;
using Terraria.UI;

namespace Terraria.Terraclient.Cheats
{
	class HandyFunctions
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
	}
}
