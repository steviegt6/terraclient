using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terraria.Terraclient.Commands
{
	public static class CommandBehaviorHelpers
	{
		public static void Output(bool IsError, string Output) {
			byte R = 15;
			byte G = 255;
			byte B = 215;
			if (IsError) {
				R = 255;
				G = 0;
				B = 0;
			}
			Main.NewText("<Terraclient> " + Output, R, G, B);
		}
	}
}
