namespace Terraria.Terraclient.Cheats
{
	public abstract class Cheat
	{
		public abstract string Name { get; }

		public virtual string Description => "";

		public virtual bool IsImportant => false;

		public virtual CheatCategory Category => CheatCategory.Misc;

		public bool IsEnabled;

		public void Toggle() => IsEnabled = !IsEnabled;
	}
}
