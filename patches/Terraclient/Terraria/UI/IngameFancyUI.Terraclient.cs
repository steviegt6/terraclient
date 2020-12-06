using Terraria.Terraclient.GameContent.UI.States;

namespace Terraria.UI
{
	public partial class IngameFancyUI
	{
		public static void OpenCheats() {
			OpenUIState(new UIManageCheats());
		}
	}
}
