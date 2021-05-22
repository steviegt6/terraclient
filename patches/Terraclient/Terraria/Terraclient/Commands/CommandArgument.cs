using System.Collections.Generic;

namespace Terraria.Terraclient.Commands
{
	public class CommandArgument
	{
		public string ArgumentName;

		public List<object> ExpectedInputs;

		public ArgInputType InputType;

		public bool MayBeSkipped;

		public enum ArgInputType
		{
			PositiveIntegerRange,
			Text,
			PositiveIntegerRangeOrText,
			TextConcatenationUntilNextInt,
			PositiveIntegerRangeOrTextConcatenationUntilNextInt,
			CustomText,
			CustomTextConcatenationUntilNextInt
		}

		public CommandArgument(string name, List<object> inputs, bool concat, bool skippable = false) {
			//Assumptions:
			/*
			First, inputs will be any of these things:
				{int, int}            NOT concat : Integer range
				{string...}           NOT concat : Expected single word
				{int, int, string...} NOT concat : Integer range OR Expected single word
				{string...}           IS  concat : Expected word or phrase, pulled from the user arguments up to the next integer or the end of the arguments
				{int, int, string...} IS  concat : Integer range OR Expected word or phrase, pulled from the user arguments up to the next integer or the end of the arguments
				{}                    NOT concat : Any single word
				{}                    IS concat : Any word or phrase, pulled from the user arguments up to the next integer or the end of the arguments
				Not choosing any of these may cause serious problems.

			the Name parameter is the display name that will be shown to the player when necessary. It should be capitalized correctly.

			the first CommandArgument that is set to Skippable in a list of CommandArguments will render all
			other CommandArguments further along the list also "skippable", regardless of their setting, because math.

			If any command would be better off handling the more raw user-input arguments by itself, the list of CommandArguments should be inputted blank when creating the command.

			Please keep in mind that when comparing strings, CheatCommandHandler.Digest() will always make all strings lowercase. This can be changed if needed, with a later patch.
			Custom text input types won't be turned lowercase before the effective argument is polished and returned by CheatCommandHandler.Digest().

			All commands that use this system should work on the assumption that the incoming arguments have been pre-processed: If syntax is wrong when a player sends in a command,
			it will be caught there. If the command does end up getting run, it should expect that all variables are ready to use.
			 */
			ArgumentName = name;
			ExpectedInputs = inputs;
			InputType = concat switch {
				false when ExpectedInputs.Count == 2 && ExpectedInputs[0] is int && ExpectedInputs[1] is int =>
					ArgInputType.PositiveIntegerRange,
				false when ExpectedInputs.Count > 0 && ExpectedInputs[0] is not int => ArgInputType.Text,
				false when ExpectedInputs.Count > 0 && ExpectedInputs[0] is int => ArgInputType
					.PositiveIntegerRangeOrText,
				true when ExpectedInputs.Count > 0 && ExpectedInputs[0] is not int => ArgInputType
					.TextConcatenationUntilNextInt,
				true when ExpectedInputs.Count > 0 && ExpectedInputs[0] is int => ArgInputType
					.PositiveIntegerRangeOrTextConcatenationUntilNextInt,
				false when ExpectedInputs.Count == 0 => ArgInputType.CustomText,
				true when ExpectedInputs.Count == 0 => ArgInputType.CustomTextConcatenationUntilNextInt,
				_ => ArgInputType.CustomText
			};
			MayBeSkipped = skippable;
		}
	}
}