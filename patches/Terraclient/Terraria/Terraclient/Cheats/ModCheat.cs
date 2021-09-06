using Terraria.ModLoader;

namespace Terraria.Terraclient.Cheats
{
	/// <summary>
	///		Base cheat class. Glorified boolean.
	/// </summary>
	public abstract class ModCheat : ModType
	{
		/// <summary>
		///		The cheat's visible name.
		/// </summary>
		public ModTranslation DisplayName { get; private set; }

		/// <summary>
		///		Text that appears when a cheat is hovered over.
		/// </summary>
		public ModTranslation Description { get; private set; }

		/// <summary>
		///		Whether this cheat renders with a large bar that takes up a whole row.
		/// </summary>
		public virtual bool LargeBar { get; set; } = false;

		/// <summary>
		///		Whether the cheat's effects are enabled.
		/// </summary>
		public virtual bool IsEnabled { get; set; }

		/// <summary>
		///		The category this cheat is a part of.
		/// </summary>
		public abstract ModCheatCategory Category { get; }

		/// <summary>
		///		Sets <see cref="IsEnabled"/> to the opposite of its current value (i.e. true -> false).
		/// </summary>
		public virtual void Toggle() => IsEnabled = !IsEnabled;

		protected sealed override void Register() {
			DisplayName = LocalizationLoader.CreateTranslation(Mod, $"CheatName.{Name}");
			Description = LocalizationLoader.CreateTranslation(Mod, $"CheatDescription.{Name}");
			ModTypeLookup<ModCheat>.Register(this);
			SetStaticDefaults();
		}
	}
}