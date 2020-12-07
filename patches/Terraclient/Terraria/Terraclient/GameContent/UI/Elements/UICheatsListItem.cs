using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.Terraclient.Cheats;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Terraria.Terraclient.GameContent.UI.Elements
{
	public class UICheatsListItem : UIElement
	{
		private Color _color;
		private readonly ICheat _cheat;

		public UICheatsListItem(Color color, ICheat cheat) {
			_cheat = cheat;
			_color = color;
			OnClick += OnClickMethod;
		}

		public void OnClickMethod(UIMouseEvent evt, UIElement listeningElement) => _cheat.SwitchEnabled();

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			base.DrawSelf(spriteBatch);

			CalculatedStyle dimensions = GetDimensions();
			Vector2 pos = new Vector2(dimensions.X, dimensions.Y);
			Vector2 defScale = new Vector2(0.8f);
			Color drawColor = _cheat.IsEnabled() ? Color.White : Color.Gray;
			drawColor = Color.Lerp(drawColor, Color.White, IsMouseHovering ? 0.5f : 0f);

			Utils.DrawSettingsPanel(spriteBatch, pos, dimensions.Width + 1f, IsMouseHovering ? _color : _color.MultiplyRGBA(new Color(180, 180, 180)));

			pos.X += 8f;
			pos.Y += 8f;

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, _cheat.Name(), pos, drawColor, 0f, Vector2.Zero, defScale, dimensions.Width + 1f);

			pos.X -= 17f;

			string text = _cheat.IsEnabled() ? "Enabled" : "Disabled";

			pos = new Vector2(dimensions.X + dimensions.Width - ChatManager.GetStringSize(FontAssets.ItemStack.Value, text, defScale).X - 10f, dimensions.Y + 8f);

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, text, pos, drawColor, 0f, Vector2.Zero, defScale, dimensions.Width + 1f);

			if (IsMouseHovering)
				Main.instance.MouseText(_cheat.Description());
		}
	}
}
