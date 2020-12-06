using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DiffPatch
{
	public class Patcher
	{
		public enum Mode
		{
			EXACT, OFFSET, FUZZY
		}

		public class Result
		{
			public Patch patch;
			public bool success;
			public Mode mode;

			public int searchOffset;
			public Patch appliedPatch;

			public int offset;
			public bool offsetWarning;
			public float fuzzyQuality;

			public string Summary() {
				if (!success)
					return $"FAILURE: {patch.Header}";

				if (mode == Mode.OFFSET)
					return (offsetWarning ? "WARNING" : "OFFSET") + $": {patch.Header} offset {offset} lines";

				if (mode == Mode.FUZZY) {
					int q = (int)(fuzzyQuality * 100);
					return $"FUZZY: {patch.Header} quality {q}%" +
						(offset > 0 ? $" offset {offset} lines" : "");
				}

				return $"EXACT: {patch.Header}";
			}
		}

		//patch extended with implementation fields
		private class WorkingPatch : Patch
		{
			public Result result;
			public string lmContext;
			public string lmPatched;
			public string[] wmContext;
			public string[] wmPatched;
			
			public WorkingPatch(Patch patch) : base(patch) {}

			public LineRange? KeepoutRange1 => result?.appliedPatch?.TrimmedRange1;
			public LineRange? KeepoutRange2 => result?.appliedPatch?.TrimmedRange2;

			public int? AppliedDelta => result?.appliedPatch?.length2 - result?.appliedPatch?.length1;

			public void Fail() {
				result = new Result {patch = this, success = false};
			}

			public void Succeed(Mode mode, Patch appliedPatch) {
				result = new Result {
					patch = this,
					success = true,
					mode = mode,
					appliedPatch = appliedPatch
				};
			}

			public void AddOffsetResult(int offset, int fileLength) {
				result.offset = offset;//note that offset is different to at - start2, because offset is relative to the applied position of the last patch
				result.offsetWarning = offset > OffsetWarnDistance(length1, fileLength);
			}

			public void AddFuzzyResult(float fuzzyQuality) {
				result.fuzzyQuality = fuzzyQuality;
			}

			public void LinesToChars(CharRepresenter rep) {
				lmContext = rep.LinesToChars(ContextLines);
				lmPatched = rep.LinesToChars(PatchedLines);
			}

			public void WordsToChars(CharRepresenter rep) {
				wmContext = ContextLines.Select(rep.WordsToChars).ToArray();
				wmPatched = PatchedLines.Select(rep.WordsToChars).ToArray();
			}
		}

		//the offset distance which constitutes a warning for a patch
		//currently either 10% of file length, or 10x patch length, whichever is longer
		public static int OffsetWarnDistance(int patchLength, int fileLength) => Math.Max(patchLength * 10, fileLength / 10);

		private readonly IReadOnlyList<WorkingPatch> patches;
		private List<string> lines;
		private bool applied;

		// Last here means highest line number, not necessarily most recent.
		// Patches can only apply before lastAppliedPatch in fuzzy mode
		private Patch lastAppliedPatch = null;

		// we maintain delta as the offset of the last patch (applied location - expected location)
		// this way if a line is inserted, and all patches are offset by 1, only the first patch is reported as offset
		// normally this is equivalent to `lastAppliedPatch?.AppliedOffset` but if a patch fails, we subtract its length delta from the search offset
		private int searchOffset;

		// patches applying within this range (due to fuzzy matching) will cause patch reordering
		private LineRange ModifiedRange => new LineRange { start = 0, end = lastAppliedPatch?.TrimmedRange2.end ?? 0 };

		private readonly CharRepresenter charRep;
		private string lmText;
		private List<string> wmLines;

		public int MaxMatchOffset { get; set; } = MatchMatrix.DefaultMaxOffset;
		public float MinMatchScore { get; set; } = FuzzyLineMatcher.DefaultMinMatchScore;

		public Patcher(IEnumerable<Patch> patches, IEnumerable<string> lines, CharRepresenter charRep = null) {
			this.patches = patches.Select(p => new WorkingPatch(p)).ToList();
			this.lines = new List<string>(lines);
			this.charRep = charRep ?? new CharRepresenter();
		}

		public void Patch(Mode mode) {
			if (applied)
				throw new Exception("Already Applied");

			applied = true;

			foreach (var patch in patches) {
				if (ApplyExact(patch))
					continue;
				if (mode >= Mode.OFFSET && ApplyOffset(patch))
					continue;
				if (mode >= Mode.FUZZY && ApplyFuzzy(patch))
					continue;

				patch.Fail();
				patch.result.searchOffset = searchOffset;
				searchOffset -= patch.length2 - patch.length1;
			}
		}

		public string[] ResultLines => lines.ToArray();
		public IEnumerable<Result> Results => patches.Select(p => p.result);

		private void LinesToChars() {
			foreach (var patch in patches)
				patch.LinesToChars(charRep);

			lmText = charRep.LinesToChars(lines);
		}

		private void WordsToChars() {
			foreach (var patch in patches)
				patch.WordsToChars(charRep);

			wmLines = lines.Select(charRep.WordsToChars).ToList();
		}

		private Patch ApplyExactAt(int loc, WorkingPatch patch) {
			if (!patch.ContextLines.SequenceEqual(lines.GetRange(loc, patch.length1)))
				throw new Exception("Patch engine failure");
			if (!CanApplySafelyAt(loc, patch))
				throw new Exception("Patch affects another patch");


			lines.RemoveRange(loc, patch.length1);
			lines.InsertRange(loc, patch.PatchedLines);

			//update the lineModeText
			if (lmText != null)
				lmText = lmText.Remove(loc) + patch.lmPatched + lmText.Substring(loc + patch.length1);

			//update the wordModeLines
			if (wmLines != null) {
				wmLines.RemoveRange(loc, patch.length1);
				wmLines.InsertRange(loc, patch.wmPatched);
			}

			int patchedDelta = patches.Where(p => p.KeepoutRange2?.end <= loc).Sum(p => p.AppliedDelta.Value);
			Patch appliedPatch = patch;
			if (appliedPatch.start2 != loc || appliedPatch.start1 != loc - patchedDelta)
				appliedPatch = new Patch(patch) { //create a new patch with different applied position if necessary
					start1 = loc - patchedDelta,
					start2 = loc
				};

			
			// update the applied location for patches following this one in the file, but preceding it in the patch list
			// can only happen if fuzzy matching causes a patch to move before one of the previously applied patches
			if (loc < ModifiedRange.end) {
				foreach (var p in patches.Where(p => p.KeepoutRange2?.start > loc))
					p.result.appliedPatch.start2 += appliedPatch.length2 - appliedPatch.length1;
			}
			else {
				lastAppliedPatch = appliedPatch;
			}

			searchOffset = appliedPatch.start2 - patch.start2;
			return appliedPatch;
		}

		private bool CanApplySafelyAt(int loc, Patch patch) {
			if (loc >= ModifiedRange.end)
				return true;

			var range = new LineRange { start = loc, length = patch.length1 };
			return patches.All(p => !p.KeepoutRange2?.Contains(range) ?? true);
		}

		private bool ApplyExact(WorkingPatch patch) {
			int loc = patch.start2 + searchOffset;
			if (loc + patch.length1 > lines.Count)
				return false;

			if (!patch.ContextLines.SequenceEqual(lines.GetRange(loc, patch.length1)))
				return false;
			
			patch.Succeed(Mode.EXACT, ApplyExactAt(loc, patch));
			return true;
		}

		private bool ApplyOffset(WorkingPatch patch) {
			if (lmText == null)
				LinesToChars();

			if (patch.length1 > lines.Count)
				return false;

			int loc = patch.start2 + searchOffset;
			if (loc < 0) loc = 0;
			else if (loc >= lines.Count) loc = lines.Count - 1;

			int forward = lmText.IndexOf(patch.lmContext, loc, StringComparison.Ordinal);
			int reverse = lmText.LastIndexOf(patch.lmContext, Math.Min(loc+patch.lmContext.Length, lines.Count-1), StringComparison.Ordinal);

			if (!CanApplySafelyAt(forward, patch))
				forward = -1;
			if (!CanApplySafelyAt(reverse, patch))
				reverse = -1;

			if (forward < 0 && reverse < 0)
				return false;

			int found = reverse < 0 || forward >= 0 && (forward - loc) < (loc - reverse) ? forward : reverse;
			patch.Succeed(Mode.OFFSET, ApplyExactAt(found, patch));
			patch.AddOffsetResult(found - loc, lines.Count);

			return true;
		}

		private bool ApplyFuzzy(WorkingPatch patch) {
			if (wmLines == null)
				WordsToChars();

			int loc = patch.start2 + searchOffset;
			if (loc + patch.length1 > wmLines.Count)//initialise search at end of file if loc is past file length
				loc = wmLines.Count - patch.length1;

			(int[] match, float matchQuality) = FindMatch(loc, patch.wmContext);
			if (match == null)
				return false;

			var fuzzyPatch = new WorkingPatch(AdjustPatchToMatchedLines(patch, match, lines));
			if (wmLines != null) fuzzyPatch.WordsToChars(charRep);
			if (lmText != null) fuzzyPatch.LinesToChars(charRep);

			int at = match.First(i => i >= 0); //if the patch needs lines trimmed off it, the early match entries will be negative
			patch.Succeed(Mode.FUZZY, ApplyExactAt(at, fuzzyPatch));
			patch.AddOffsetResult(fuzzyPatch.start2 - loc, lines.Count);
			patch.AddFuzzyResult(matchQuality);
			return true;
		}

		public static Patch AdjustPatchToMatchedLines(Patch patch, int[] match, IReadOnlyList<string> lines) {
			//replace the patch with a copy
			var fuzzyPatch = new Patch(patch);
			var diffs = fuzzyPatch.diffs; //for convenience

			//keep operations, but replace lines with lines in source text
			//unmatched patch lines (-1) are deleted
			//unmatched target lines (increasing offset) are added to the patch
			for (int i = 0, j = 0, ploc = -1; i < patch.length1; i++) {
				int mloc = match[i];

				//insert extra target lines into patch
				if (mloc >= 0 && ploc >= 0 && mloc - ploc > 1) {
					//delete an unmatched target line if the surrounding diffs are also DELETE, otherwise use it as context
					var op = diffs[j - 1].op == Operation.DELETE && diffs[j].op == Operation.DELETE ?
						 Operation.DELETE : Operation.EQUAL;

					for (int l = ploc + 1; l < mloc; l++)
						diffs.Insert(j++, new Diff(op, lines[l]));
				}
				ploc = mloc;

				//keep insert lines the same
				while (diffs[j].op == Operation.INSERT)
					j++;

				if (mloc < 0) //unmatched context line
					diffs.RemoveAt(j);
				else //update context to match target file (may be the same, doesn't matter)
					diffs[j++].text = lines[mloc];
			}

			//finish our new patch
			fuzzyPatch.RecalculateLength();
			return fuzzyPatch;
		}

		private (int[] match, float score) FindMatch(int loc, IReadOnlyList<string> wmContext) {
			// fuzzy matching is more complex because we need to split up the patched file to only search _between_ previously applied patches
			var keepoutRanges = patches.Select(p => p.KeepoutRange2).Where(r => r != null).Select(r => r.Value);

			// parts of file to search in
			var ranges = new LineRange { length = wmLines.Count }.Except(keepoutRanges).ToArray();

			return FuzzyMatch(wmContext, wmLines, loc, MaxMatchOffset, MinMatchScore, ranges);
		}
		
		public static (int[] match, float score) FuzzyMatch(IReadOnlyList<string> wmPattern, IReadOnlyList<string> wmText, int loc, int maxMatchOffset = MatchMatrix.DefaultMaxOffset, float minMatchScore = FuzzyLineMatcher.DefaultMinMatchScore, LineRange[] ranges = default) {
			if (ranges == null)
				ranges = new LineRange[] { new LineRange { length = wmText.Count } };

			// we're creating twice as many MatchMatrix objects as we need, incurring some wasted allocation and setup time, but it reads easier than trying to precompute all the edge cases
			var fwdMatchers = ranges.Select(r => new MatchMatrix(wmPattern, wmText, maxMatchOffset, r)).SkipWhile(m => loc > m.WorkingRange.last).ToArray();
			var revMatchers = ranges.Reverse().Select(r => new MatchMatrix(wmPattern, wmText, maxMatchOffset, r)).SkipWhile(m => loc < m.WorkingRange.first).ToArray();

			int warnDist = OffsetWarnDistance(wmPattern.Count, wmText.Count);
			float penaltyPerLine = 1f / (10*warnDist);

			var fwd = new MatchRunner(loc, 1, fwdMatchers, penaltyPerLine);
			var rev = new MatchRunner(loc,-1, revMatchers, penaltyPerLine);

			float bestScore = minMatchScore;
			int[] bestMatch = null;
			while (fwd.Step(ref bestScore, ref bestMatch) | rev.Step(ref bestScore, ref bestMatch));

			return (bestMatch, bestScore);
		}

		private struct MatchRunner
		{
			private int loc;
			private readonly int dir;
			private readonly MatchMatrix[] mms;
			private readonly float penaltyPerLine;

			// used as a Range/Slice for the MatchMatrix array
			private LineRange active;
			private float penalty;

			public MatchRunner(int loc, int dir, MatchMatrix[] mms, float penaltyPerLine) {
				this.loc = loc;
				this.dir = dir;
				this.mms = mms;
				this.penaltyPerLine = penaltyPerLine;
				active = new LineRange();
				penalty = -0.1f; // start penalty at -10%, to give some room for finding the best match if it's not "too far"
			}

			public bool Step(ref float bestScore, ref int[] bestMatch) {
				if (active.first == mms.Length)
					return false;

				if (bestScore > 1f - penalty)
					return false; //aint getting any better than this

				// activate matchers as we enter their working range
				while (active.end < mms.Length && mms[active.end].WorkingRange.Contains(loc))
					active.end++;

				// active MatchMatrix runs
				for (int i = active.first; i <= active.last; i++) {
					var mm = mms[i];
					if (!mm.Match(loc, out float score)) {
						Debug.Assert(i == active.first, "Match matricies out of order?");
						active.first++;
						continue;
					}

					if (penalty > 0) //ignore penalty for the first 10%
						score -= penalty;

					if (score > bestScore) {
						bestScore = score;
						bestMatch = mm.Path();
					}
				}

				loc += dir;
				penalty += penaltyPerLine;

				return true;
			}
		}
	}
}
