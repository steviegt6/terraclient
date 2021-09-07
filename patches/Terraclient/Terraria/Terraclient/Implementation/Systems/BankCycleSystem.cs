using System;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.Terraclient.Loading;

namespace Terraria.Terraclient.Implementation.Systems
{
	[TerraclientAssociated]
	public class BankCycleSystem : ModPlayer
	{
		public enum CycleDirection
		{
			Up,
			Down
		}

		public static int SpecialContainerType { get; protected set; } = -1;

		public override void ProcessTriggers(TriggersSet triggersSet) {
			base.ProcessTriggers(triggersSet);
		}

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
	}
}