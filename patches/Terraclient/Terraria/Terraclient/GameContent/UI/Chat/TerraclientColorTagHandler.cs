using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System.Globalization;
using Terraria.GameContent;
using Terraria.Terraclient.Commands;
using Terraria.UI.Chat;

namespace Terraria.Terraclient.GameContent.UI.Chat
{
	public class TerraclientColorTagHandler : ITagHandler
	{
		public class TerraclientNameSnippet : TextSnippet
		{
			public bool IsError { get; }

			public TerraclientNameSnippet(bool isError) {
				IsError = isError;
			}

			public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch,
				Vector2 position = default, Color color = default, float scale = 1, bool isShadow = false) {
				const string drawText = "<Terraclient>";
				char[] terraclientChars = drawText.ToCharArray();
				Color[] colors = new Color[terraclientChars.Length];
				DynamicSpriteFont font = FontAssets.MouseText.Value;
				size = new Vector2();

				if (IsError)
					for (int i = 0; i < terraclientChars.Length; i++)
						colors[i] = ColorFromHex(CheatCommandUtils.ErrorColors[i]);
				else
					for (int i = 0; i < terraclientChars.Length; i++)
						colors[i] = ColorFromHex(CheatCommandUtils.NonErrorColors[i]);

				for (int i = 0; i < terraclientChars.Length; i++) {
					string singleChar = terraclientChars[i].ToString();

					if (!justCheckingString && !isShadow)
						ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, singleChar, position, colors[i],
							0f, Vector2.Zero, Vector2.One);

					float increase = font.MeasureString(singleChar).X;
					size.X += increase;
					position.X += increase;
				}

				return false;
			}

			private static Color ColorFromHex(string hexCode) => int.TryParse(hexCode, NumberStyles.AllowHexSpecifier,
				CultureInfo.InvariantCulture, out int hex)
				? new Color((hex >> 16) & 0xFF, (hex >> 8) & 0xFF, hex & 0xFF)
				: Color.White;
		}

		public TextSnippet Parse(string text, Color baseColor = default, string options = null) =>
			bool.TryParse(text, out bool value)
				? new TerraclientNameSnippet(value)
				: new TerraclientNameSnippet(false);
	}
}