using Terraria.GameContent.Bestiary;
using Terraria.Terraclient.GameContent.UI.Elements;
using Terraria.UI;

namespace Terraria.Terraclient.GameContent.Bestiary
{
	public class SpawnButtonBestiaryElement : IBestiaryInfoElement
	{
		public int NPC { get; protected set; }

		public SpawnButtonBestiaryElement(int npc) {
			NPC = npc;
		}

		public UIElement ProvideUIElement(BestiaryUICollectionInfo info) => new UIBestiarySpawnButtonLine(NPC);
	}
}