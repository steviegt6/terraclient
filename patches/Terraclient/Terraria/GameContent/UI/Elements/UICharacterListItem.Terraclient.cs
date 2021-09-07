using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace Terraria.GameContent.UI.Elements
{
	public partial class UICharacterListItem
	{
		private void CycleMouseOver(UIMouseEvent evt, UIElement listeningElement) {
			_buttonLabel.SetText(Language.GetTextValue("Terraclient.UICyclePlayerDifficulty"));
		}

		private void CycleDifficulty(UIMouseEvent mouseEvent, UIElement element) {
			SoundEngine.PlaySound(SoundID.MenuTick);

			switch (_data.Player.difficulty) {
				case 0:
				case 1:
				case 2:
					_data.Player.difficulty++;
					break;

				case 3:
					_data.Player.difficulty = 0;
					break;
			}

			Player.SavePlayer(_data);
			Main.OpenCharacterSelectUI();
			(Parent.Parent as UIList)?.UpdateOrder();
		}
	}
}