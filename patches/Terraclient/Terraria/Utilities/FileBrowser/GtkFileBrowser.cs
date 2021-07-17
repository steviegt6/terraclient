using Gtk;

namespace Terraria.Utilities.FileBrowser
{
	public class GtkFileBrowser : Dialog, IFileBrowser
	{
		public GtkFileBrowser() : base("Select file", GtkWindow.GetWindow(), DialogFlags.DestroyWithParent,
			FileChooserAction.Open, Stock.Close) {
		}

		public string OpenFilePanel(string title, ExtensionFilter[] extensions) {
			FileChooserDialog dialog = new FileChooserDialog("", this, FileChooserAction.Open,
				FileChooserAction.Open, Stock.Cancel);
			PopulateFilters(ref dialog, extensions);

			if (dialog.Run() == (int)ResponseType.Ok) {
				string fileName = dialog.Filename;
				dialog.Destroy();
				return fileName;
			}
			
			dialog.Destroy();
			return null;
			
		}

		public static void PopulateFilters(ref FileChooserDialog chooser, ExtensionFilter[] extensions) {
			foreach (ExtensionFilter filter in extensions) {
				chooser.AddFilter(GenerateFilter(filter));
			}
		}

		public static FileFilter GenerateFilter(ExtensionFilter eFilter) {
			FileFilter filter = new FileFilter();

			foreach (string eFilterPattern in eFilter.Extensions) {
				if (eFilterPattern.StartsWith("*"))
					filter.AddPattern(eFilterPattern);
				else if (eFilterPattern.StartsWith("."))
					filter.AddPattern($".{eFilterPattern}");
				else
					filter.AddPattern($"*.{eFilterPattern}");
			}

			return filter;
		}
	}
}