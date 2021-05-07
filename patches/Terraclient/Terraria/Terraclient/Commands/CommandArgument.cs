using System.Collections.Generic;

namespace Terraria.Terraclient.Commands
{
	public class CommandArgument
	{
		public string ArgumentName;

		public List<object> ExpectedInputs;

		public byte InputType;

		public bool MayBeSkipped;

		public const byte PositiveIntegerRange = 0;

		public const byte Text = 1;

		public const byte TextConcatenationUntilNextInt = 2;

		public const byte PositiveIntegerRangeOrTextConcatenationUntilNextInt = 3;

		public const byte CustomText = 4;

		public const byte CustomTextConcatenationUntilNextInt = 5;

		public CommandArgument(string name, List<object> inputs, bool concat, bool skippable = false) {
			//Assumptions:
			/*
			First, inputs will be any of these things:
			If Concat is true,
				it is possible that it is {}: Custom concat reaching the next integer or the end of the argument list.
				it is possible that it is {int, int, string...}: Either a positive integer range (the first two ints) or any one of the inputs after that.
				it is possible that it is {string...}: Concat that may match any of these values (or autocomplete to them).
			or,	then
				it is possible that it is {int, int}: A range of expected integers, clamped to this minimum and maximum.
				it is possible that it is {}: Any single word of text.
				it is possible that it is {string...}: A single word of text that may match any of these values (or autocomplete to them).

			the Name parameter is the display name that will be shown to the player when necessary. It should be capitalized.

			the first CommandArgument that is set to Skippable in a list of CommandArguments will render all
			other CommandArguments further along the list also "skippable", regardless of their setting, because math.

			If any command would be better off handling the more raw userinputted arguments by itself, the list of CommandArguments should be inputted blank when creating the command.

			Please keep in mind that when comparing strings, CheatCommandHandler.Digest() will always make all strings lowercase. This can be changed if needed, with a later patch.
			Custom text input types won't be turned lowercase before the effective argument is polished and returned by CheatCommandHandler.Digest().

			All commands that use this system should work on the assumption that the incoming arguments have been pre-processed: If syntax is wrong when a player sends in a command,
			it will be caught there. If the command does end up getting run, it should expect that all variables are ready to use.
			 */
			ArgumentName = name;
			ExpectedInputs = inputs;
			if (concat) {
				if (inputs.Count == 0)
					InputType = CustomTextConcatenationUntilNextInt;
				else if (inputs[0] is int && inputs[1] is int)
					InputType = PositiveIntegerRangeOrTextConcatenationUntilNextInt;
				else
					InputType = TextConcatenationUntilNextInt;
			}
			else {
				if (inputs.Count == 2 && inputs[0] is int && inputs[1] is int)
					InputType = PositiveIntegerRange;
				else if (inputs.Count == 0)
					InputType = CustomText;
				else
					InputType = Text;
			}
			MayBeSkipped = skippable;
		}
	}
}