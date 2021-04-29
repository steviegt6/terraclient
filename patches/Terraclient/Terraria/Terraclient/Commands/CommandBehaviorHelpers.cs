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
			if (!IsError) {
				Main.NewText("[c/fc7303:<]" +
					"[c/8aff9e:T]" +
					"[c/8affa7:e]" +
					"[c/8affb1:r]" +
					"[c/8affbb:r]" +
					"[c/8affc5:a]" +
					"[c/8affce:c]" +
					"[c/8affd8:l]" +
					"[c/8affe2:i]" +
					"[c/8affeb:e]" +
					"[c/8afff5:n]" +
					"[c/8affff:t]" +
					"[c/fc7303:>] " + Output, 138, 255, 206);
				return;
			}
			Main.NewText("[c/8a8a8a:<]" +
					"[c/f23d2c:T]" +
					"[c/eb3b2a:e]" +
					"[c/e33929:r]" +
					"[c/db3727:r]" +
					"[c/d43526:a]" +
					"[c/cc3325:c]" +
					"[c/c43123:l]" +
					"[c/bd2f22:i]" +
					"[c/bd2f22:e]" +
					"[c/c43123:n]" +
					"[c/cc3325:t]" +
					"[c/8a8a8a:>] " + Output, 235, 59, 42);
		}
	}
}
