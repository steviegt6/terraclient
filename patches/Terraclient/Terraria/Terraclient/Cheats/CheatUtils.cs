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

			bool foundPick = false;
			bool foundAxe = false;
			bool foundHammer = false;
			foreach (Item item in Main.LocalPlayer.inventory) {
				if (!foundPick && item.pick > 0)
					goto foundPick;
				if (!foundAxe && item.axe > 0)
					goto foundAxe;
				if (!foundHammer && item.hammer > 0)
					goto foundHammer;
				if (item.pick > 0 || item.axe > 0 || item.hammer > 0) {
					item.Refresh();
					ResetItemName(item);
				}
				continue;
			foundPick:
				foundPick = true;
				item.pick = 1000;
				if (!foundAxe && item.axe > 0)
					goto foundAxe;
				if (!foundHammer && item.hammer > 0)
					goto foundHammer;
				item.useTime = 0;
				item.useAnimation = 7;
				item.tileBoost = 15;
				MarkItemAsModified(item);
				continue;
			foundAxe:
				foundAxe = true;
				item.axe = 100;
				if (!foundHammer && item.hammer > 0)
					goto foundHammer;
				item.useTime = 0;
				item.useAnimation = 7;
				item.tileBoost = 15;
				MarkItemAsModified(item);
				continue;
			foundHammer:
				foundHammer = true;
				item.hammer = 1000;
				item.useTime = 0;
				item.useAnimation = 7;
				item.tileBoost = 15;
				MarkItemAsModified(item);
			}
			if (Main.mouseItem.pick > 0) {
				Main.mouseItem.pick = 500;
			}
			if (Main.mouseItem.axe > 0) {
				Main.mouseItem.axe = 100;
			}
			if (Main.mouseItem.hammer > 0) {
				Main.mouseItem.hammer = 100;
			}
			if (Main.mouseItem.pick > 0 || Main.mouseItem.axe > 0 || Main.mouseItem.hammer > 0) {
				Main.mouseItem.useTime = 0;
				Main.mouseItem.useAnimation = 7;
				Main.mouseItem.tileBoost = 15;
				MarkItemAsModified(Main.mouseItem);
			}
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

			Main.LocalPlayer.maxMinions = CheatHandler.TerraclientMaxMinionsOverride;

			if (!CheatHandler.GetCheat<RefillsCheat>().IsEnabled)
				return;

			foreach (Item item in Main.LocalPlayer.inventory)
				if (!item.IsAir && item.favorited && item.stack < item.maxStack)
					item.stack = item.maxStack;

			foreach (Item item in Main.LocalPlayer.miscEquips)
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

		public static void MarkItemAsModified(Item item) => item.SetNameOverride(ContentSamples.ItemsByType[item.type].AffixName() + "*");

		public static void ResetItemName(Item item) => item.ClearNameOverride();

		public static void ResetItemNames() {
			Item[][] arr = {
				Main.LocalPlayer.inventory,
				Main.LocalPlayer.armor,
				Main.LocalPlayer.dye,
				Main.LocalPlayer.miscEquips,
				Main.LocalPlayer.miscDyes,
				Main.LocalPlayer.bank.item,
				Main.LocalPlayer.bank2.item,
				Main.LocalPlayer.bank3.item,
				Main.LocalPlayer.bank4.item
			};

			for (int i = 0; i < arr.Length; i++)
				for (int j = 0; j < arr[i].Length; j++)
					ResetItemName(arr[i][j]);
		}
	}
}