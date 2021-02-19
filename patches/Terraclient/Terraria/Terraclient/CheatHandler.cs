using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.IO;
using Terraria.Terraclient.Cheats;

namespace Terraria.Terraclient
{
	public static class CheatHandler
	{
		public static Preferences CheatConfiguration => new Preferences(CheatPath + Path.DirectorySeparatorChar + "cheat_config.json");

		public static string CheatPath => Main.SavePath + Path.DirectorySeparatorChar + "Terraclient";

		internal static List<Cheat> cheats;

		public static Cheat GetCheat<T>() where T : Cheat => cheats.First(c => c.GetType() == typeof(T));

		public static void SaveCheatSettings() {
			CheatConfiguration.Clear();

			foreach (Cheat cheat in cheats)
				CheatConfiguration.Put(cheat.GetType().Name, cheat.IsEnabled);

			CheatConfiguration.Save();
		}

		public static void LoadCheatSettings() {
			InitializeCheats();
			CheatConfiguration.Load();

			foreach (Cheat cheat in cheats) {
				bool value = cheat.IsEnabled;
				CheatConfiguration.Get(cheat.GetType().Name, ref value);
				cheat.IsEnabled = value;
			}

			CheatConfiguration.Save();
		}

		private static void InitializeCheats() {
			if (cheats != null)
				return;

			cheats = new List<Cheat>();

			foreach (Type type in typeof(CheatHandler).Assembly.GetTypes()) {
				if (type.IsAbstract || type.GetConstructor(new Type[] { }) == null || !type.IsSubclassOf(typeof(Cheat)) || !(Activator.CreateInstance(type) is Cheat cheat))
					continue;

				cheats.Add(cheat);
			}

			cheats = cheats.OrderBy(c => c.GetType().Name).ToList();
		}
	}
}
