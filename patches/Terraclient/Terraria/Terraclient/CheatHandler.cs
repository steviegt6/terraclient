using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Terraclient.Cheats;
using Terraria.Terraclient.Commands;

namespace Terraria.Terraclient
{
	public static class CheatHandler
	{
		public static Version TCVersion = new(0, 1, 0, 0);

		internal static List<Cheat> cheats;

		public static Cheat GetCheat<T>() where T : Cheat => cheats.First(c => c.GetType() == typeof(T));

		internal static void InitializeCheats() {
			if (cheats != null)
				return;

			cheats = new List<Cheat>();

			foreach (Type type in typeof(CheatHandler).Assembly.GetTypes()) {
				if (type.IsAbstract || type.GetConstructor(Array.Empty<Type>()) == null || !type.IsSubclassOf(typeof(Cheat)) || !(Activator.CreateInstance(type) is Cheat cheat))
					continue;

				cheats.Add(cheat);
			}

			cheats = cheats.OrderBy(c => c.GetType().Name).ToList();
		}

		internal static void DoUpdate_Terraclient() {
			CheatCommandHandler.UpdateColors();
		}
	}
}
