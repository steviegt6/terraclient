using System.Collections.Generic;

namespace DiffPatch
{
	public class PatienceDiffer : Differ
	{
		public string LineModeString1 { get; private set; }
		public string LineModeString2 { get; private set; }

		public PatienceDiffer(CharRepresenter charRep = null) : base(charRep) { }

		public override int[] Match(IReadOnlyList<string> lines1, IReadOnlyList<string> lines2) {
			LineModeString1 = charRep.LinesToChars(lines1);
			LineModeString2 = charRep.LinesToChars(lines2);
			return new PatienceMatch().Match(LineModeString1, LineModeString2, charRep.MaxLineChar);
		}
	}
}