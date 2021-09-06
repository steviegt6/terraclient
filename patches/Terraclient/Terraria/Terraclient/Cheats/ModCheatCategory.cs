using Terraria.ModLoader;

namespace Terraria.Terraclient.Cheats
{
	// TODO: Allow the changing of colors and stuff. :p
	/// <summary>
	///		Base category class for cheats.
	/// </summary>
	public abstract class ModCheatCategory : ModType
	{
		public ModTranslation DisplayName { get; private set; }

		protected sealed override void Register() {
			DisplayName = LocalizationLoader.CreateTranslation(Mod, $"CheatCategories.{Name}");
			ModTypeLookup<ModCheatCategory>.Register(this);
			SetStaticDefaults();
		}
	}
}