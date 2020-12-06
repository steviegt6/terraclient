using System;
using System.Collections.Generic;
using System.Linq;

namespace DiffPatch
{
	public class Patch
	{
		public List<Diff> diffs;
		public int start1;
		public int start2;
		public int length1;
		public int length2;

		public Patch() {
			diffs = new List<Diff>();
		}

		public Patch(Patch patch) {
			diffs = new List<Diff>(patch.diffs.Select(d => new Diff(d.op, d.text)));
			start1 = patch.start1;
			start2 = patch.start2;
			length1 = patch.length1;
			length2 = patch.length2;
		}

		public string Header => $"@@ -{start1 + 1},{length1} +{start2 + 1},{length2} @@";
		public string AutoHeader => $"@@ -{start1 + 1},{length1} +_,{length2} @@";

		public IEnumerable<string> ContextLines => diffs.Where(d => d.op != Operation.INSERT).Select(d => d.text);
		public IEnumerable<string> PatchedLines => diffs.Where(d => d.op != Operation.DELETE).Select(d => d.text);
		public LineRange Range1 => new LineRange {start = start1, length = length1};
		public LineRange Range2 => new LineRange {start = start2, length = length2};

		public LineRange TrimmedRange1 => TrimRange(Range1);
		public LineRange TrimmedRange2 => TrimRange(Range2);

		private LineRange TrimRange(LineRange range) {
			int start = 0;
			while (start < diffs.Count && diffs[start].op == Operation.EQUAL)
				start++;

			if (start == diffs.Count)
				return new LineRange { start = range.start, length = 0};

			int end = diffs.Count;
			while (end > start && diffs[end - 1].op == Operation.EQUAL)
				end--;

			return new LineRange { start = range.start + start, end = range.end - (diffs.Count - end)};
		}

		public void RecalculateLength() {
			length1 = diffs.Count;
			length2 = diffs.Count;
			foreach (var d in diffs)
				if (d.op == Operation.DELETE) length2--;
				else if (d.op == Operation.INSERT) length1--;
		}

		public override string ToString() => Header + Environment.NewLine +
									string.Join(Environment.NewLine, diffs);

		public void Trim(int numContextLines) {
			var r = TrimRange(new LineRange{ start = 0, length = diffs.Count });

			if (r.length == 0) {
				length1 = length2 = 0;
				diffs.Clear();
				return;
			}

			int trimStart = r.start - numContextLines;
			int trimEnd = diffs.Count - r.end - numContextLines;
			if (trimStart > 0) {
				diffs.RemoveRange(0, trimStart);
				start1 += trimStart;
				start2 += trimStart;
				length1 -= trimStart;
				length2 -= trimStart;
			}

			if (trimEnd > 0) {
				diffs.RemoveRange(diffs.Count - trimEnd, trimEnd);
				length1 -= trimEnd;
				length2 -= trimEnd;
			}
		}

		public void Uncollate() {
			var uncollatedDiffs = new List<Diff>(diffs.Count);
			var addDiffs = new List<Diff>();
			foreach (var d in diffs) {
				if (d.op == Operation.DELETE) {
					uncollatedDiffs.Add(d);
				}
				else if (d.op == Operation.INSERT) {
					addDiffs.Add(d);
				}
				else {
					uncollatedDiffs.AddRange(addDiffs);
					addDiffs.Clear();
					uncollatedDiffs.Add(d);
				}
			}
			uncollatedDiffs.AddRange(addDiffs); //patches may not end with context diffs
			diffs = uncollatedDiffs;
		}

		public List<Patch> Split(int numContextLines) {
			if (diffs.Count == 0)
				return new List<Patch>();

			var ranges = new List<LineRange>();
			int start = 0;
			int n = 0;
			for (int i = 0; i < diffs.Count; i++) {
				if (diffs[i].op == Operation.EQUAL) {
					n++;
					continue;
				}

				if (n > numContextLines * 2) {
					ranges.Add(new LineRange {start = start, end = i - n + numContextLines});
					start = i - numContextLines;
				}

				n = 0;
			}

			ranges.Add(new LineRange {start = start, end = diffs.Count});

			var patches = new List<Patch>(ranges.Count);
			int end1 = start1, end2 = start2;
			int endDiffIndex = 0;
			foreach (var r in ranges) {
				int skip = r.start - endDiffIndex;
				var p = new Patch {
					start1 = end1 + skip,
					start2 = end2 + skip,
					diffs = diffs.Slice(r).ToList()
				};
				p.RecalculateLength();
				patches.Add(p);
				end1 = p.start1 + p.length1;
				end2 = p.start2 + p.length2;
				endDiffIndex = r.end;
			}

			return patches;
		}


		public void Combine(Patch patch2, IReadOnlyList<string> lines1) {
			if (Range1.Intersects(patch2.Range1) || Range2.Intersects(patch2.Range2))
				throw new ArgumentException("Patches overlap");

			while (start1 + length1 < patch2.start1) {
				diffs.Add(new Diff(Operation.EQUAL, lines1[start1 + length1]));
				length1++;
				length2++;
			}

			if (start2 + length2 != patch2.start2)
				throw new ArgumentException("Unequal distance between end of patch1 and start of patch2 in context and patched");

			diffs.AddRange(patch2.diffs);
			length1 += patch2.length1;
			length2 += patch2.length2;
		}
	}
}
