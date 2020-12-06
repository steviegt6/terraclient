using System.Collections.Generic;

namespace DiffPatch
{
	public sealed class PatienceMatch
	{
		//working fields for matching
		private string chars1;
		private string chars2;
		private int[] unique1;
		private int[] unique2;
		private int[] matches;

		private void Match(int start1, int end1, int start2, int end2) {
			// step 1: match up identical starting lines
			while (start1 < end1 && start2 < end2 && chars1[start1] == chars2[start2])
				matches[start1++] = start2++;

			// step 2: match up identical ending lines
			while (start1 < end1 && start2 < end2 && chars1[end1 - 1] == chars2[end2 - 1])
				matches[--end1] = --end2;

			if (start1 == end1 || start2 == end2 || //no lines on a side
				end1 - start1 + end2 - start2 <= 3)//either a 1-2 or 2-1 which would've been matched by steps 1 and 2
				return;

			// step 3: match up common unique lines
			bool any = false;
			foreach (var (m1, m2) in LCSUnique(start1, end1, start2, end2)) {
				matches[m1] = m2;
				any = true;

				//step 4: recurse
				Match(start1, m1, start2, m2);

				start1 = m1 + 1; start2 = m2 + 1;
			}

			if (any)
				Match(start1, end1, start2, end2);
		}

		private int[] Match() {
			matches = new int[chars1.Length];
			for (int i = 0; i < chars1.Length; i++)
				matches[i] = -1;

			Match(0, chars1.Length, 0, chars2.Length);
			return matches;
		}

		public int[] Match(string chars1, string chars2, int maxChar) {
			if (unique1 == null || unique1.Length < maxChar) {
				unique1 = new int[maxChar];
				unique2 = new int[maxChar];
				for (int i = 0; i < maxChar; i++)
					unique1[i] = unique2[i] = -1;
			}

			this.chars1 = chars1;
			this.chars2 = chars2;
			
			return Match();
		}
		
		private readonly List<int> subChars = new List<int>();
		private IEnumerable<(int, int)> LCSUnique(int start1, int end1, int start2, int end2) {
			//identify all the unique chars in chars1
			for (int i = start1; i < end1; i++) {
				int c = chars1[i];

				if (unique1[c] == -1) {//no lines
					unique1[c] = i;
					subChars.Add(c);
				}
				else {
					unique1[c] = -2;//not unique
				}
			}

			//identify all the unique chars in chars2, provided they were unique in chars1
			for (int i = start2; i < end2; i++) {
				int c = chars2[i];
				if (unique1[c] < 0)
					continue;
				
				unique2[c] = unique2[c] == -1 ? i : -2;
			}

			//extract common unique subsequences
			var common1 = new List<int>();
			var common2 = new List<int>();
			foreach (int i in subChars) {
				if (unique1[i] >= 0 && unique2[i] >= 0) {
					common1.Add(unique1[i]);
					common2.Add(unique2[i]);
				}
				unique1[i] = unique2[i] = -1; //reset for next use
			}
			subChars.Clear();

			if (common2.Count == 0)
				yield break;

			// repose the longest common subsequence as longest ascending subsequence
			// note that common2 is already sorted by order of appearance in file1 by of char allocation
			foreach (int i in LASIndices(common2))
				yield return (common1[i], common2[i]);
		}

		private class LCANode
		{
			public readonly int value;
			public readonly LCANode prev;

			public LCANode(int value, LCANode prev) {
				this.value = value;
				this.prev = prev;
			}
		}

		//https://en.wikipedia.org/wiki/Patience_sorting
		public static int[] LASIndices(IReadOnlyList<int> sequence) {
			if (sequence.Count == 0)
				return new int[0];
			
			var pileTops = new List<LCANode> {new LCANode(0, null)};
			for (int i = 1; i < sequence.Count; i++) {
				int v = sequence[i];

				//binary search for the first pileTop > v
				int a = 0;
				int b = pileTops.Count;
				while (a != b) {
					int c = (a + b) / 2;
					if (sequence[pileTops[c].value] > v)
						b = c;
					else
						a = c + 1;
				}

				if (a < pileTops.Count)
					pileTops[a] = new LCANode(i, a > 0 ? pileTops[a - 1] : null);
				else
					pileTops.Add(new LCANode(i, pileTops[a - 1]));
			}

			//follow pointers back through path
			var las = new int[pileTops.Count];
			int j = pileTops.Count - 1;
			for (var node = pileTops[j]; node != null; node = node.prev)
				las[j--] = node.value;

			return las;
		}
	}
}
