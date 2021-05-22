using Terraria.Terraclient.Cheats.General;
using Terraria.UI;
using Terraria.ID;

namespace Terraria.Terraclient.Cheats
{
	public static class CheatUtils
	{
		public static int HeavyTasksTimer { get; private set; }

		public static bool TryDupe(Item slot, bool overrideCheatCheck = false) {
			if (!CheatHandler.GetCheat<AltRightClickDuplicationCheat>().IsEnabled && !overrideCheatCheck)
				return false;

			if (Main.cursorOverride != 3)
				return false;

			if (!Main.mouseRight || !Main.mouseRightRelease ||
			    (!(Main.mouseItem.IsTheSameAs(slot) | Main.mouseItem.IsAir)))
				return true;

			Main.mouseItem = new Item();
			Main.mouseItem.SetDefaults(slot.type);
			Main.mouseItem.Prefix(slot.prefix);

			if (ItemSlot.ControlInUse && Main.mouseItem.maxStack != 1)
				Main.mouseItem.stack = int.MaxValue;
			else
				Main.mouseItem.stack = Main.mouseItem.maxStack;

			return true;
		}

		public static void ToolGodBuffMyTools(bool overrideCheatCheck = false) {
			if (!CheatHandler.GetCheat<ToolGodCheat>().IsEnabled && !overrideCheatCheck)
				return;

			int i;
			for (i = 0; i < Main.player[Main.myPlayer].inventory.Length; i++) {
				if (Main.player[Main.myPlayer].inventory[i].pick <= 0)
					continue;

				Main.player[Main.myPlayer].inventory[i].pick = ContentSamples.ItemsByType[2786].pick;
				Main.player[Main.myPlayer].inventory[i].useTime = 0;
				Main.player[Main.myPlayer].inventory[i].useAnimation = 7;
				Main.player[Main.myPlayer].inventory[i].tileBoost = 15;
				break;
			}

			for (i++; i < Main.player[Main.myPlayer].inventory.Length; i++)
				if (Main.player[Main.myPlayer].inventory[i].pick > 0)
					Main.player[Main.myPlayer].inventory[i].Refresh();

			for (i = 0; i < Main.player[Main.myPlayer].inventory.Length; i++) {
				if (Main.player[Main.myPlayer].inventory[i].axe <= 0)
					continue;

				Main.player[Main.myPlayer].inventory[i].axe = ContentSamples.ItemsByType[1305].axe;
				Main.player[Main.myPlayer].inventory[i].useTime = 0;
				Main.player[Main.myPlayer].inventory[i].useAnimation = 7;
				Main.player[Main.myPlayer].inventory[i].tileBoost = 15;
				break;
			}

			for (i++; i < Main.player[Main.myPlayer].inventory.Length; i++)
				if (Main.player[Main.myPlayer].inventory[i].axe > 0)
					Main.player[Main.myPlayer].inventory[i].Refresh();

			for (i = 0; i < Main.player[Main.myPlayer].inventory.Length; i++) {
				if (Main.player[Main.myPlayer].inventory[i].hammer <= 0)
					continue;

				Main.player[Main.myPlayer].inventory[i].hammer = ContentSamples.ItemsByType[1305].hammer;
				Main.player[Main.myPlayer].inventory[i].useTime = 0;
				Main.player[Main.myPlayer].inventory[i].useAnimation = 7;
				Main.player[Main.myPlayer].inventory[i].tileBoost = 4;
				break;
			}

			for (i++; i < Main.player[Main.myPlayer].inventory.Length; i++)
				if (Main.player[Main.myPlayer].inventory[i].hammer > 0)
					Main.player[Main.myPlayer].inventory[i].Refresh();
		}

		public static void ResetEffectsMod() {
			switch (HeavyTasksTimer) {
				case > 0:
					HeavyTasksTimer--;
					break;

				case 0:
					HeavyTasksTimer = 8;
					ToolGodBuffMyTools();
					break;
			}

			if (!CheatHandler.GetCheat<RefillsCheat>().IsEnabled)
				return;

			foreach (Item item in Main.player[Main.myPlayer].inventory)
				if (!item.IsAir && item.favorited && item.stack < item.maxStack)
					item.stack = item.maxStack;

			foreach (Item item in Main.player[Main.myPlayer].miscEquips)
				if (!item.IsAir && item.favorited && item.stack < item.maxStack)
					item.stack = item.maxStack;
		}

		public static void MoveMouseItemToInventory() {
			if (Main.mouseItem.type <= 0)
				return;

			Main.mouseItem.position = Main.player[Main.myPlayer].Center;
			if (Main.mouseItem.stack > 0) {
				int itemInstanceID = Item.NewItem((int)Main.player[Main.myPlayer].position.X,
					(int)Main.player[Main.myPlayer].position.Y,
					Main.player[Main.myPlayer].width,
					Main.player[Main.myPlayer].height,
					Main.mouseItem.type,
					Main.mouseItem.stack,
					false,
					Main.mouseItem.prefix,
					false,
					true);
				Main.item[itemInstanceID].newAndShiny = false;

				if (Main.netMode == 1)
					NetMessage.SendData(21, -1, -1, null, itemInstanceID, 1f);
			}

			Main.mouseItem = new Item();
			Main.player[Main.myPlayer].inventory[58] = new Item();
			Recipe.FindRecipes();
		}

		public static void MarkItemAsModified(Item item) => item.SetNameOverride(item.HoverName + "*");

		public static void ResetItemName(Item item) => item.ClearNameOverride();
	}
}