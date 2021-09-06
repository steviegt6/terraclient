using Microsoft.Xna.Framework;
using System;
using Terraria.Audio;
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
			OnMouseDown += SpawnNPC;
			BorderColor = new Color(89, 116, 213, 255);

			UIText panel = new UIText(Language.GetTextValue("Terraclient.BestiarySpawnButton")) {
				HAlign = 0.5f, 
				VAlign = 0.5f, 
				IgnoresMouseInteraction = true
			};
			Append(panel);
		}

		private void SpawnNPC(UIMouseEvent evt, UIElement listeningElement) {
			switch (Main.netMode) {
				case 0:
					Terraria.NPC.NewNPC((int)Main.LocalPlayer.position.X, (int)Main.LocalPlayer.position.Y - 200, NPC);
					break;

				case 1:
					NetMessage.SendData(130, -1, -1, null,
						(int)Math.Floor(Main.LocalPlayer.position.X / 16f),
						(float)Math.Floor((Main.LocalPlayer.position.Y - 200) / 16f),
						NPC);
					break;
			}
			//Doesn't work well on TShock. There, it only summons specific NPCs after a short timer once a fishing rod is pulled by the player.
		}

		private void MouseOver(UIMouseEvent evt, UIElement listeningElement) {
			SoundEngine.PlaySound(12);
			BorderColor = Colors.FancyUIFatButtonMouseOver;
		}

		private void MouseOut(UIMouseEvent evt, UIElement listeningElement) =>
			BorderColor = new Color(89, 116, 213, 255);
	}
}