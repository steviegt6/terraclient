using Gtk;

namespace Terraria.Utilities.FileBrowser
{
	public class GtkWindow : Window
	{
		private GtkWindow() : base(WindowType.Toplevel) {
			Title = "File selection window";
			SetDefaultSize(350, 250);
			DeleteEvent += (_, _) => Application.Quit();
			Show();
		}

		internal static GtkWindow GetWindow() {
			Application.Init();
			GtkWindow window = new GtkWindow();
			Application.Run();
			return window;
		}
	}
}