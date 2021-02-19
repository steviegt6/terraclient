using Terraria.Localization;

namespace Terraria.Terraclient.Cheats
{
	public abstract class Cheat
	{
		public virtual string Name => Language.GetTextValue($"Cheats.{GetType().Name}Name");

		public virtual string Description => Language.GetTextValue($"Cheats.{GetType().Name}Desc");

		public virtual bool IsImportant => false;

		public virtual CheatCategory Category => CheatCategory.Misc;

		public bool isEnabled;

		public void Toggle() => isEnabled = !isEnabled;
	}
}
