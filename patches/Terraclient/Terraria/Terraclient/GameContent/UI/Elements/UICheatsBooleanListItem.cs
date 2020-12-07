using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Terraria.Terraclient.GameContent.UI.Elements
{
	public class UICheatsBooleanListItem : UIElement
	{
		private Color _color;
		private readonly FieldInfo _boolean;

		public UICheatsBooleanListItem(Color color, FieldInfo boolean) {
			_color = color;
			OnClick += OnClickMethod;
		}

		public void OnClickMethod(UIMouseEvent evt, UIElement listeningElement) => _boolean.SetValue(null, !(bool)_boolean.GetValue(null));

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			base.DrawSelf(spriteBatch);

			CalculatedStyle dimensions = GetDimensions();
			Vector2 pos = new Vector2(dimensions.X, dimensions.Y);
			Vector2 defScale = new Vector2(0.8f);
			Color drawColor = (bool)_boolean.GetValue(null) ? Color.White : Color.Gray;
			drawColor = Color.Lerp(drawColor, Color.White, IsMouseHovering ? 0.5f : 0f);

			Utils.DrawSettingsPanel(spriteBatch, pos, dimensions.Width + 1f, IsMouseHovering ? _color : _color.MultiplyRGBA(new Color(180, 180, 180)));

			pos.X += 8f;
			pos.Y += 8f;

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, Language.GetTextValue($"Cheats.{_boolean.Name}Name"), pos, drawColor, 0f, Vector2.Zero, defScale, dimensions.Width + 1f);

			string text = (bool)_boolean.GetValue(null) ? Language.GetTextValue("UI.CheatEnabled") : Language.GetTextValue("UI.CheatDisabled");

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, text, new Vector2(dimensions.X + dimensions.Width - ChatManager.GetStringSize(FontAssets.ItemStack.Value, text, defScale).X - 10f, dimensions.Y + 8f), drawColor, 0f, Vector2.Zero, defScale, dimensions.Width + 1f);

			if (IsMouseHovering)
				Main.instance.MouseText(Language.GetTextValue($"Cheats.{_boolean.Name}Desc"));
		}
	}
}
