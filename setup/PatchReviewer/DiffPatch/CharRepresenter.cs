using System;
using System.Collections.Generic;
using System.Linq;

namespace DiffPatch
{
	public class CharRepresenter
	{
		private readonly List<string> charToLine = new List<string>();
		private readonly Dictionary<string, char> lineToChar = new Dictionary<string, char>();

		private readonly List<string> charToWord = new List<string>();
		private readonly Dictionary<string, char> wordToChar = new Dictionary<string, char>();

		public int MaxLineChar => charToLine.Count;
		public int MaxWordChar => charToWord.Count;

		public CharRepresenter() {

			charToLine.Add("\0");//lets avoid the 0 char

			//keep ascii chars as their own values
			for (int i = 0; i < 0x80; i++)
				charToWord.Add(((char)i).ToString());
		}

		public char AddLine(string line) {
			if (!lineToChar.TryGetValue(line, out char c)) {
				lineToChar.Add(line, c = (char)charToLine.Count);
				charToLine.Add(line);
			}

			return c;
		}

		public char AddWord(string word) {
			if (word.Length == 1 && word[0] <= 0x80)
				return word[0];

			if (!wordToChar.TryGetValue(word, out char c)) {
				wordToChar.Add(word, c = (char)charToWord.Count);
				charToWord.Add(word);
			}

			return c;
		}

		private char[] buf = new char[4096];
		public virtual string WordsToChars(string line) {
			int b = 0;

			for (int i = 0, len; i < line.Length; i += len) {
				char c = line[i];
				//identify word
				len = 1;
				if (char.IsLetter(c)) while (i + len < line.Length && char.IsLetterOrDigit(line, i + len)) len++;
				else if (char.IsDigit(c)) while (i + len < line.Length && char.IsDigit(line, i + len)) len++;
				else if (c == ' ' || c == '\t') while (i + len < line.Length && line[i + len] == c) len++;
				string word = line.Substring(i, len);

				if (b > buf.Length) Array.Resize(ref buf, buf.Length * 2);
				buf[b++] = AddWord(word);
			}

			return new string(buf, 0, b);
		}

		public string LinesToChars(IEnumerable<string> lines) => new string(lines.Select(AddLine).ToArray());

		public string GetWord(int c) => charToWord[c];
	}
}
