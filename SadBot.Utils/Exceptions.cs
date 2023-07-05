// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

namespace SadBot.Utils;

public class InvalidArgumentException : Exception
{
	public string Argument { get; init; }

	public InvalidArgumentException(string argument) : base($"Invalid argument '{argument}'.")
	{
		Argument = argument;
	}

	public InvalidArgumentException(string argument, string message) : base(message)
	{
		Argument = argument;
	}
}
