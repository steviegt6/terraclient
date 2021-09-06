using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.UI;

namespace Terraria.Terraclient.GameContent.UI.Elements
{
	public class UIBuffIcon : UIElement
	{
		public Asset<Texture2D> BuffTexture { get; }

		public UIBuffIcon(int buff) {
			BuffTexture = TextureAssets.Buff[buff];
			Width.Set(32f, 0f);
			Height.Set(32f, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) =>
			spriteBatch.Draw(BuffTexture.Value, GetDimensions().Center(), BuffTexture.Frame(), Color.White, 0f,
				BuffTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
	}
}