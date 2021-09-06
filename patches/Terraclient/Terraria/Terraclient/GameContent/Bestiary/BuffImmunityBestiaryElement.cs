using Terraria.GameContent.Bestiary;
using Terraria.Terraclient.GameContent.UI.Elements;
using Terraria.UI;

namespace Terraria.Terraclient.GameContent.Bestiary
{
	public class BuffImmunityBestiaryElement : IBestiaryInfoElement
	{
		public int Buff { get; protected set; }

		public BuffImmunityBestiaryElement(int buff) {
			Buff = buff;
		}

		public UIElement ProvideUIElement(BestiaryUICollectionInfo info) =>
			info.UnlockState < BestiaryEntryUnlockState.CanShowDropsWithDropRates_4
				? null
				: new UIBestiaryInfoBuffImmunityLine(Buff, info);
	}
}