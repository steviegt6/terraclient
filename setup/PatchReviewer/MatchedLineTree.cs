using System.Collections.Generic;
using System.Linq;
using RedBlack;
using DiffPatch;
using CountAccessor = RedBlack.RedBlackCountAccesor<PatchReviewer.MatchedLineNode>;
using System;

namespace PatchReviewer
{
	public sealed class MatchedLineNode : RedBlackNode<MatchedLineNode>
	{
		public bool HasLeftLine { get; private set; }
		public bool HasRightLine { get; private set; }

		public int lineCount, leftLineCount, rightLineCount;

		public bool SidesEqual { get; private set; }
		public IList<LineRange> leftDiffRanges, rightDiffRanges;

		public MatchedLineNode(bool hasLeftLine, bool hasRightLine) {
			HasLeftLine = hasLeftLine;
			HasRightLine = hasRightLine;
			ChildrenChanged();
		}

		public override void ChildrenChanged() {
			int _lineCount = 1;
			int _leftLineCount = HasLeftLine ? 1 : 0;
			int _rightLineCount = HasRightLine ? 1 : 0;
			if (Left != null) {
				_lineCount += Left.lineCount;
				_leftLineCount += Left.leftLineCount;
				_rightLineCount += Left.rightLineCount;
			}
			if (Right != null) {
				_lineCount += Right.lineCount;
				_leftLineCount += Right.leftLineCount;
				_rightLineCount += Right.rightLineCount;
			}
			if (lineCount != _lineCount || leftLineCount != _leftLineCount || rightLineCount != _rightLineCount) {
				lineCount = _lineCount;
				rightLineCount = _rightLineCount;
				leftLineCount = _leftLineCount;
				Parent?.ChildrenChanged();
			}
		}

		public int LineCount(bool r) => r ? rightLineCount : leftLineCount;
		public bool HasLine(bool r) => r ? HasRightLine : HasLeftLine;
		public void SetHasLine(bool r, bool has) {
			if (r) HasRightLine = has;
			else HasLeftLine = has;

			leftDiffRanges = rightDiffRanges = null;
			ChildrenChanged();
		}

		public void SetEqual() {
			if (!HasRightLine || !HasLeftLine)
				throw new ArgumentException("Cannot be equal without having lines for both sides");

			SidesEqual = true;
			leftDiffRanges = rightDiffRanges = null;
		}

		public void Compare(int[] matches, string wmLeft, string wmRight, CharRepresenter charRep) {
			SidesEqual = false;
			if (!HasRightLine || !HasLeftLine)
				throw new ArgumentException("Cannot be compared without having lines for both sides");

			if (matches.Count(i => i < 0) > matches.Length / 2f) { //not similar enough
				leftDiffRanges = rightDiffRanges = null;
				return;
			}

			leftDiffRanges = new List<LineRange>();
			rightDiffRanges = new List<LineRange>();

			int i1 = 0, i2 = 0;
			int offset1 = 0, offset2 = 0;

			foreach (var (range1, range2) in LineMatching.UnmatchedRanges(matches, wmRight.Length)) {
				while (i1 < range1.start)
					offset1 += charRep.GetWord(wmLeft[i1++]).Length;
				while (i2 < range2.start)
					offset2 += charRep.GetWord(wmRight[i2++]).Length;

				int start1 = offset1, start2 = offset2;
				while (i1 < range1.end)
					offset1 += charRep.GetWord(wmLeft[i1++]).Length;
				while (i2 < range2.end)
					offset2 += charRep.GetWord(wmRight[i2++]).Length;

				leftDiffRanges.Add(new LineRange { start = start1, end = offset1 });
				rightDiffRanges.Add(new LineRange { start = start2, end = offset2 });
			}
		}

		public IList<LineRange> DiffRanges(bool r) => r ? rightDiffRanges : leftDiffRanges;

		public MatchedLineNode GetCopy => new MatchedLineNode(HasLeftLine, HasRightLine) {
			leftDiffRanges = leftDiffRanges,
			rightDiffRanges = rightDiffRanges,
			SidesEqual = SidesEqual
		};
	}

	public class MatchedLineTree : BaseRedBlackTree<MatchedLineNode>, IReadOnlyList<MatchedLineNode>
	{
		public class SideAccess
		{
			public readonly MatchedLineTree tree;
			public readonly bool side;

			public SideAccess(MatchedLineTree tree, bool side) {
				this.tree = tree;
				this.side = side;
			}
			
			public int Count => tree.Root?.LineCount(side) ?? 0;

			public MatchedLineNode this[int i] =>
				CountAccessor.GetByIndex(tree, i, n => n.LineCount(side));
			
			public int IndexOf(MatchedLineNode node) =>
				CountAccessor.IndexOf(tree, node, n => n.LineCount(side));
			
			public int OppositeIndexOf(MatchedLineNode node) =>
				CountAccessor.IndexOf(tree, node, n => n.LineCount(!side));
			
			public int CombinedIndexOf(int lineNo) => lineNo == Count ? tree.Count : CombinedIndexOf(this[lineNo]);
			public int CombinedIndexOf(MatchedLineNode node) =>
				CountAccessor.IndexOf(tree, node, n => n.lineCount);

			public void Recompare(MatchedLineNode node, string ourLine, string otherLine) {
				if (ourLine == otherLine) {
					node.SetEqual();
					return;
				}

				var charRep = new CharRepresenter();
				var wmLeft = charRep.WordsToChars(side ? otherLine : ourLine);
				var wmRight = charRep.WordsToChars(side ? ourLine : otherLine);
				var match = new PatienceMatch().Match(wmLeft, wmRight, charRep.MaxWordChar);
				node.Compare(match, wmLeft, wmRight, charRep);
			}
		}

		public readonly SideAccess leftAccessor;
		public readonly SideAccess rightAccessor;

		public MatchedLineTree(int[] matches, int rightLineCount) {
			BuildFrom(ToNodes(matches, rightLineCount));
			leftAccessor = new SideAccess(this, false);
			rightAccessor = new SideAccess(this, true);
		}

		public SideAccess Access(bool side) => side ? rightAccessor : leftAccessor;

		public int IndexOf(MatchedLineNode node) => CountAccessor.IndexOf(this, node, n => n.lineCount);
		public MatchedLineNode this[int index] => CountAccessor.GetByIndex(this, index, n => n.lineCount);

		public IEnumerable<MatchedLineNode> Slice(LineRange range) => 
			range.length == 0 ? Enumerable.Empty<MatchedLineNode>() : 
				this[range.first].To(this[range.last]);

		private static IReadOnlyList<MatchedLineNode> ToNodes(int[] matches, int rightLineCount) {
			var list = new List<MatchedLineNode>(matches.Length);
			int right = 0;
			foreach (var m in matches) {
				if (m == -1) {
					list.Add(new MatchedLineNode(true, false));
					continue;
				}
				while (right < m) {
					list.Add(new MatchedLineNode(false, true));
					right++;
				}
				list.Add(new MatchedLineNode(true, true));
				right++;
			}

			while (right < rightLineCount){
				list.Add(new MatchedLineNode(false, true));
				right++;
			}

			return list;
		}

		private void CompareMatched(IReadOnlyList<string> wmLeft, IReadOnlyList<string> wmRight, CharRepresenter charRep) {
			var matcher = new PatienceMatch();

			int leftLineNo = 0, rightLineNo = 0;
			foreach (var line in this) {
				if (line.HasLeftLine && line.HasRightLine) {
					string left = wmLeft[leftLineNo], right = wmRight[rightLineNo];
					if (left == right)
						line.SetEqual();
					else {
						var match = matcher.Match(left, right, charRep.MaxWordChar);
						line.Compare(match, left, right, charRep);
					}
				}

				if (line.HasLeftLine) leftLineNo++;
				if (line.HasRightLine) rightLineNo++;
			}
		}

		public static MatchedLineTree FromLines(IReadOnlyList<string> leftLines, IReadOnlyList<string> rightLines) {
			var lmDiff = new LineMatchedDiffer { MinMatchScore = 0 };
			var matchedLines = new MatchedLineTree(lmDiff.Match(leftLines, rightLines), rightLines.Count);
			matchedLines.CompareMatched(lmDiff.WordModeLines1, lmDiff.WordModeLines2, lmDiff.charRep);
			return matchedLines;
		}

		public static int[] ToMatches(IEnumerable<MatchedLineNode> nodes, int leftLength) {
			var matches = Enumerable.Repeat(-1, leftLength).ToArray();
			int i = 0, j = 0;
			foreach (var node in nodes) {
				if (node.SidesEqual)
					matches[i] = j;

				if (node.HasLeftLine) i++;
				if (node.HasRightLine) j++;
			}

			return matches;
		}
	}
}
