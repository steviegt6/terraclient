using System;
using System.Collections.Generic;

namespace DiffPatch
{
	public static class LineMatching
	{
		public static IEnumerable<(LineRange, LineRange)> UnmatchedRanges(int[] matches, int len2) {
			int len1 = matches.Length;
			int start1 = 0, start2 = 0;
			do {
				//search for a matchpoint
				int end1 = start1;
				while (end1 < len1 && matches[end1] < 0)
					end1++;

				int end2 = end1 == len1 ? len2 : matches[end1];
				if (end1 != start1 || end2 != start2) {
					yield return (new LineRange { start = start1, end = end1 }, new LineRange { start = start2, end = end2 });
					start1 = end1;
					start2 = end2;
				} else {//matchpoint follows on from start, no unmatched lines
					start1++;
					start2++;
				}
			} while (start1 < len1 || start2 < len2);
		}

		public static int[] FromUnmatchedRanges(IEnumerable<(LineRange, LineRange)> unmatchedRanges, int len1) {
			int[] matches = new int[len1];
			int start1 = 0, start2 = 0;
			foreach (var (range1, range2) in unmatchedRanges) {
				while (start1 < range1.start)
					matches[start1++] = start2++;

				if (start2 != range2.start)
					throw new ArgumentException("Unequal number of lines between umatched ranges on each side");

				while (start1 < range1.end)
					matches[start1++] = -1;

				start2 = range2.end;
			}

			while (start1 < len1)
				matches[start1++] = start2++;

			return matches;
		}

		public static IEnumerable<(LineRange, LineRange)> UnmatchedRanges(IEnumerable<Patch> patches) {
			foreach (var patch in patches) {
				var diffs = patch.diffs;
				int start1 = patch.start1, start2 = patch.start2;
				for (int i = 0; i < diffs.Count;) {
					//skip matched
					while (i < diffs.Count && diffs[i].op == Operation.EQUAL) {
						start1++;
						start2++;
						i++;
					}

					int end1 = start1, end2 = start2;
					while (i < diffs.Count && diffs[i].op != Operation.EQUAL) {
						if (diffs[i++].op == Operation.DELETE)
							end1++;
						else
							end2++;
					}

					if (end1 != start1 || end2 != start2)
						yield return (new LineRange { start = start1, end = end1 }, new LineRange { start = start2, end = end2 });

					start1 = end1;
					start2 = end2;
				}
			}
		}

		public static int[] FromPatches(IEnumerable<Patch> patches, int len1) => 
			FromUnmatchedRanges(UnmatchedRanges(patches), len1);

		public static List<Diff> MakeDiffList(int[] matches, IReadOnlyList<string> lines1, IReadOnlyList<string> lines2) {
			var list = new List<Diff>();
			int l = 0, r = 0;
			for (int i = 0; i < matches.Length; i++) {
				if (matches[i] < 0)
					continue;

				while (l < i)
					list.Add(new Diff(Operation.DELETE, lines1[l++]));

				while (r < matches[i])
					list.Add(new Diff(Operation.INSERT, lines2[r++]));

				if (lines1[l] != lines2[r]) {
					list.Add(new Diff(Operation.DELETE, lines1[l]));
					list.Add(new Diff(Operation.INSERT, lines2[r]));
				}
				else {
					list.Add(new Diff(Operation.EQUAL, lines1[l]));
				}
				l++; r++;
			}

			while (l < lines1.Count)
				list.Add(new Diff(Operation.DELETE, lines1[l++]));
			
			while (r < lines2.Count)
				list.Add(new Diff(Operation.INSERT, lines2[r++]));

			return list;
		}
	}
}
