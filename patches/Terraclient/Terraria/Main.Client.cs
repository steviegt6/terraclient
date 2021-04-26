using System;
using Terraria.GameInput;
using Terraria.Localization;

namespace Terraria
{
	public partial class Main
	{
		public enum CycleDirection
		{
			Up,
			Down
		}

		public static int SpecialContainerType { get; private set; } = -1;

		public static string CurrentLoadStage { get; internal set; } = Language.GetTextValue("LoadCycles.Waiting");

		public static void CycleContainerType(CycleDirection direction) {
			if (SpecialContainerType == -1) {
				SpecialContainerType = -2;
				return;
			}

			switch (direction) {
				case CycleDirection.Up:
					switch (SpecialContainerType) {
						case -5:
						case -4:
						case -3:
							SpecialContainerType++;
							break;

						case -2:
							SpecialContainerType = -5;
							break;
					}
					break;

				case CycleDirection.Down:
					switch (SpecialContainerType) {
						case -2:
						case -3:
						case -4:
							SpecialContainerType--;
							break;

						case -5:
							SpecialContainerType = -2;
							break;
					}
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}
		}

		private static void HandleClientInput() {
			bool bankChanged = false;

			if (PlayerInput.Triggers.Current.CycleBankUp) {
				CycleContainerType(CycleDirection.Up);
				PlayerInput.Triggers.Current.CycleBankUp = false;
				bankChanged = true;
			}

			if (PlayerInput.Triggers.Current.CycleBankDown) {
				CycleContainerType(CycleDirection.Down);
				PlayerInput.Triggers.Current.CycleBankDown = false;
				bankChanged = true;
			}

			if (!bankChanged)
				return;

			player[myPlayer].chest = SpecialContainerType;
			playerInventory = true;
		}
	}
}