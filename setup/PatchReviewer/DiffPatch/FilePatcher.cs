using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DiffPatch
{
	public class FilePatcher
	{
		public string patchFilePath;
		public PatchFile patchFile;
		public string rootDir = "";
		public string[] baseLines, patchedLines;
		public List<Patcher.Result> results;
		
		public string BasePath => Path.Combine(rootDir, patchFile.basePath);
		public string PatchedPath => Path.Combine(rootDir, patchFile.patchedPath);

		public void LoadBaseFile() {
			baseLines = File.ReadAllLines(BasePath);
		}

		public void Patch(Patcher.Mode mode) {
			if (baseLines == null)
				LoadBaseFile();

			var patcher = new Patcher(patchFile.patches, baseLines);
			patcher.Patch(mode);
			results = patcher.Results.ToList();
			patchedLines = patcher.ResultLines;
		}

		public void Save() {
			File.WriteAllLines(PatchedPath, patchedLines);
		}

		public static FilePatcher FromPatchFile(string patchFilePath, string rootDir = "") {
			return new FilePatcher {
				patchFilePath = patchFilePath,
				patchFile = PatchFile.FromText(File.ReadAllText(patchFilePath)),
				rootDir = rootDir
			};
		}
	}
}
