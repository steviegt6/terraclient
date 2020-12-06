namespace DiffPatch
{
	public class Diff
	{
		public Operation op;
		public string text;
			
		public Diff(Operation op, string text) {
			this.op = op;
			this.text = text;
		}

		public override string ToString() => 
			(op == Operation.EQUAL ? ' ' :
			op == Operation.INSERT ? '+' : '-') + text;
	}
}
