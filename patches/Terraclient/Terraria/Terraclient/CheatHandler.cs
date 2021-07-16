using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.Terraclient.Cheats;
using Terraria.Terraclient.Commands;

namespace Terraria.Terraclient
{
	public static class CheatHandler
	{
		public static string Version => TerraclientVersion + TerraclientVersionEx;

		private static readonly Version TerraclientVersion = new(0, 0, 1, 0);
		private const string TerraclientVersionEx = " alpha-21w22a";

		internal static List<Cheat> Cheats;

		public static Cheat GetCheat<T>() where T : Cheat => Cheats.First(c => c.GetType() == typeof(T));

		internal static void PreLaunch() {
			// Register CodePagesEncodingProvider for ZIPs
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		}

		internal static void InitializeCheats() {
			if (Cheats != null)
				return;

			Cheats = new List<Cheat>();

			foreach (Type type in typeof(CheatHandler).Assembly.GetTypes()) {
				if (type.IsAbstract || type.GetConstructor(Array.Empty<Type>()) == null ||
				    !type.IsSubclassOf(typeof(Cheat)) || !(Activator.CreateInstance(type) is Cheat cheat))
					continue;

				Cheats.Add(cheat);
			}

			Cheats = Cheats.OrderBy(c => c.GetType().Name).ToList();
		}

		internal static void DoUpdate_Terraclient() => CheatCommandHandler.UpdateColors();
	}
}