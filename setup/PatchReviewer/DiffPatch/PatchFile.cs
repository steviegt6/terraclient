using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DiffPatch
{
	public class PatchFile
	{
		private static readonly Regex HunkOffsetRegex = new Regex(@"@@ -(\d+),(\d+) \+([_\d]+),(\d+) @@", RegexOptions.Compiled);

		public string basePath;
		public string patchedPath;

		public List<Patch> patches = new List<Patch>();

		public bool IsEmpty => patches.Count == 0;

		public static PatchFile FromText(string patchText, bool verifyHeaders = true) => 
			FromLines(patchText.Split('\n').Select(l => l.TrimEnd('\r')), verifyHeaders);

		public static PatchFile FromLines(IEnumerable<string> lines, bool verifyHeaders = true) {
			var patchFile = new PatchFile();
			Patch patch = null;
			int delta = 0;

			int i = 0;
			foreach (var line in lines) {
				i++;

				//ignore blank lines
				if (line.Length == 0)
					continue;

				//context
				if (patch == null && line[0] != '@') {
					if (i == 1 && line.StartsWith("--- "))
						patchFile.basePath = line.Substring(4);
					else if (i == 2 && line.StartsWith("+++ "))
						patchFile.patchedPath = line.Substring(4);
					else
						throw new ArgumentException($"Invalid context line:{line}");

					continue;
				}

				switch (line[0]) {
					case '@':
						var m = HunkOffsetRegex.Match(line);
						if (!m.Success)
							throw new ArgumentException($"Invalid patch line {i}:{line}");

						patch = new Patch {
							start1 = int.Parse(m.Groups[1].Value) - 1,
							length1 = int.Parse(m.Groups[2].Value),
							length2 = int.Parse(m.Groups[4].Value)
						};

						//auto calc applied offset
						if (m.Groups[3].Value == "_") {
							patch.start2 = patch.start1 + delta;
						} else {
							patch.start2 = int.Parse(m.Groups[3].Value) - 1;
							if (verifyHeaders && patch.start2 != patch.start1 + delta)
								throw new ArgumentException($"Applied Offset Mismatch. Expected: {patch.start1 + delta + 1}, Actual: {patch.start2 + 1}");
						}

						delta += patch.length2 - patch.length1;
						patchFile.patches.Add(patch);
						break;
					case ' ':
						patch.diffs.Add(new Diff(Operation.EQUAL, line.Substring(1)));
						break;
					case '+':
						patch.diffs.Add(new Diff(Operation.INSERT, line.Substring(1)));
						break;
					case '-':
						patch.diffs.Add(new Diff(Operation.DELETE, line.Substring(1)));
						break;
					default:
						throw new ArgumentException($"Invalid patch line {i}:{line}");
				}
			}
			
			if (verifyHeaders) {
				foreach (var p in patchFile.patches) {
					if (p.length1 != p.ContextLines.Count())
						throw new ArgumentException($"Context length doesn't match contents: {p.Header}");
					if (p.length2 != p.PatchedLines.Count())
						throw new ArgumentException($"Patched length doesn't match contents: {p.Header}");
				}
			}

			return patchFile;
		}

		public string ToString(bool autoOffset = false) {
			var sb = new StringBuilder();
			if (basePath != null && patchedPath != null) {
				sb.Append("--- ").AppendLine(basePath);
				sb.Append("+++ ").AppendLine(patchedPath);
			}

			foreach (var p in patches) {
				sb.AppendLine(autoOffset ? p.AutoHeader : p.Header);
				foreach (var diff in p.diffs)
					sb.AppendLine(diff.ToString());
			}

			return sb.ToString();
		}
	}
}
