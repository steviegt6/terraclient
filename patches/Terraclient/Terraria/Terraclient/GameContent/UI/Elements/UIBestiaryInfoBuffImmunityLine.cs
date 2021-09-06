using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace Terraria.Terraclient.GameContent.UI.Elements
{
	public class UIBestiaryInfoBuffImmunityLine : UIPanel, IManuallyOrderedUIElement
	{
		public int Buff { get; }

		public int OrderInUIList { get; set; }

		public UIBestiaryInfoBuffImmunityLine(int buff, BestiaryUICollectionInfo info, float textScale = 1f) {
			Buff = buff;

			string buffText = Lang.GetBuffName(Buff);
			PaddingBottom = 0;
			PaddingLeft = 10f;
			PaddingRight = 10f;
			PaddingTop = 0;
			Width.Set(-14f, 1f);
			Height.Set(32f, 0f);
			Left.Set(5f, 0f);
			OnMouseOver += MouseOver;
			OnMouseOut += MouseOut;
			BorderColor = new Color(89, 116, 213, 255);

			UIBuffIcon buffIcon = new UIBuffIcon(Buff) {
				IgnoresMouseInteraction = true,
				HAlign = 0f,
				Left = new StyleDimension(4f, 0f)
			};
			Append(buffIcon);

			UITextPanel<string> textPanel = new UITextPanel<string>(buffText, textScale) {
				IgnoresMouseInteraction = true,
				DrawPanel = false,
				HAlign = 1f,
				Top = new StyleDimension(-4f, 0f)
			};
			Append(textPanel);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			base.DrawSelf(spriteBatch);

			if (IsMouseHovering)
				DrawMouseOver();
		}

		public override int CompareTo(object obj) =>
			(IManuallyOrderedUIElement)obj != null
				? OrderInUIList.CompareTo(((IManuallyOrderedUIElement)obj).OrderInUIList)
				: base.CompareTo(obj);

		public void DrawMouseOver() {
			Main.instance.MouseText(Lang.GetBuffDescription(Buff));
			Main.mouseText = true;
		}

		private void MouseOver(UIMouseEvent evt, UIElement listeningElement) {
			SoundEngine.PlaySound(12);
			BorderColor = Colors.FancyUIFatButtonMouseOver;
		}

		private void MouseOut(UIMouseEvent evt, UIElement listeningElement) =>
			BorderColor = new Color(89, 116, 213, 255);
	}
}