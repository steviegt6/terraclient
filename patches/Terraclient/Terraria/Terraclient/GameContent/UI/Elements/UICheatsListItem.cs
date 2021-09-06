using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.Terraclient.Cheats;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Terraria.Terraclient.GameContent.UI.Elements
{
	public class UICheatsListItem : UIElement
	{
		public Color Color { get; protected set; }

		public ModCheat Cheat { get; protected set; }

		public UICheatsListItem(Color color, ModCheat cheat) {
			Color = color;
			Cheat = cheat;

			OnClick += OnClickMethod;
		}

		public void OnClickMethod(UIMouseEvent evt, UIElement listeningElement) => Cheat.Toggle();

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			base.DrawSelf(spriteBatch);

			CalculatedStyle dimensions = GetDimensions();
			Vector2 pos = new Vector2(dimensions.X, dimensions.Y);
			Vector2 defScale = new Vector2(0.8f);
			Color drawColor = Cheat.IsEnabled ? Color.White : Color.Gray;
			drawColor = Color.Lerp(drawColor, Color.White, IsMouseHovering ? 0.5f : 0f);

			Utils.DrawSettingsPanel(spriteBatch, pos, dimensions.Width + 1f,
				IsMouseHovering ? Color : Color.MultiplyRGBA(new Color(180, 180, 180)));

			pos.X += 8f;
			pos.Y += 8f;

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value,
				Cheat.DisplayName.GetTranslation(Language.ActiveCulture), pos,
				drawColor, 0f, Vector2.Zero, defScale, dimensions.Width + 1f);

			string text = Cheat.IsEnabled
				? Language.GetTextValue("Terraclient.UICheatEnabled")
				: Language.GetTextValue("Terraclient.UICheatDisabled");

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, text,
				new Vector2(
					dimensions.X + dimensions.Width -
					ChatManager.GetStringSize(FontAssets.ItemStack.Value, text, defScale).X - 10f, dimensions.Y + 8f),
				drawColor, 0f, Vector2.Zero, defScale, dimensions.Width + 1f);

			if (IsMouseHovering)
				Main.instance.MouseText(Cheat.Description.GetTranslation(Language.ActiveCulture));
		}
	}
}