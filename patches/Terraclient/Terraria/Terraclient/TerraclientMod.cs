using System.IO;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Terraclient.Cheats;

namespace Terraria.Terraclient
{
	public abstract class TerraclientMod : Mod
	{
		public virtual Preferences CheatConfigFile { get; }

		protected TerraclientMod() {
			// ReSharper disable once VirtualMemberCallInConstructor
			CheatConfigFile = new Preferences(Path.Combine(Main.SavePath, "Terraclient", Name));
		}

		public override void PostSetupContent() {
			base.PostSetupContent();

			// Load config after cheats are added.
			LoadConfig();
		}

		/// <summary>
		///		Saves current cheat configurations.
		/// </summary>
		public virtual void SaveConfig() {
			CheatConfigFile.Clear();

			foreach (ModCheat cheat in GetContent<ModCheat>())
				CheatConfigFile.Put(cheat.Name, cheat.IsEnabled);

			CheatConfigFile.Save();
		}

		/// <summary>
		///		Loads current cheat configurations.
		/// </summary>
		public virtual void LoadConfig() {
			CheatConfigFile.Load();

			foreach (ModCheat cheat in GetContent<ModCheat>())
				cheat.IsEnabled = CheatConfigFile.Get(cheat.Name, false);
		}
	}
}