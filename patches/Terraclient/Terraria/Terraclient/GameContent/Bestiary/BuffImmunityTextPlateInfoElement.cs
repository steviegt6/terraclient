using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace Terraria.Terraclient.GameContent.Bestiary
{
	public class BuffImmunityTextPlateInfoElement : IBestiaryInfoElement
	{
		public UIElement ProvideUIElement(BestiaryUICollectionInfo info) {
			UIElement main = new UIElement {
				Width = new StyleDimension(0f, 1f),
				Height = new StyleDimension(24f, 0f)
			};

			UIElement sub = new UIText(Language.GetText("Terraclient.BestiaryImmuneTo")) {
				HAlign = 0.5f, VAlign = 0.5f, Top = new StyleDimension(2f, 0f), IgnoresMouseInteraction = true
			};

			main.Append(sub);
			return main;
		}
	}
}