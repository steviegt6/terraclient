using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace Terraria.Terraclient.GameContent.UI.Elements
{
	public class UIBestiarySpawnButtonLine : UIPanel, IManuallyOrderedUIElement
	{
		public int NPC { get; }

		public int OrderInUIList { get; set; }

		private Color _hoverColor = Color.White;

		public UIBestiarySpawnButtonLine(int npc) {
			NPC = npc;

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

			UIText panel =
				new UIText(Language.GetTextValue("Bestiary.SpawnButton")) {
					HAlign = 0.5f, VAlign = 0.5f, IgnoresMouseInteraction = true
				};
			Append(panel);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			base.DrawSelf(spriteBatch);

			OnClick += SpawnNPC;
		}

		private void SpawnNPC(UIMouseEvent evt, UIElement listeningElement) {
			// Single-player-only
			if (Main.netMode == 0)
				Terraria.NPC.NewNPC((int)Main.LocalPlayer.position.X, (int)Main.LocalPlayer.position.Y - 200, NPC);
		}

		private void MouseOver(UIMouseEvent evt, UIElement listeningElement) {
			SoundEngine.PlaySound(12);
			BorderColor = Colors.FancyUIFatButtonMouseOver;
			_hoverColor = IsMouseHovering ? Color.Goldenrod : Color.White;
		}

		private void MouseOut(UIMouseEvent evt, UIElement listeningElement) =>
			BorderColor = new Color(89, 116, 213, 255);
	}
}