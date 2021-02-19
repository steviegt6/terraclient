namespace Terraria.Terraclient.Cheats
{
	public abstract class Cheat
	{
		public abstract string Name { get; }

		public virtual string Description => "";

		public virtual bool IsEnabled { get; set; } = false;

		public virtual bool IsImportant => false;

		public virtual CheatCategory Category => CheatCategory.Misc;

		public void Toggle() { }
	}
}
