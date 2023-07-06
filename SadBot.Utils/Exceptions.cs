// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

namespace SadBot.Utils;

public class UtilsException : Exception
{
	public string ModuleName { get; init; }

	public UtilsException(string moduleName) : base("Utils library exception occurred.")
	{
		ModuleName = moduleName;
	}

	public UtilsException(string moduleName, string message) : base(message)
	{
		ModuleName = moduleName;
	}
}

public sealed class InvalidArgumentException : UtilsException
{
	public string Argument { get; init; }

	public InvalidArgumentException(string argument) : base("Args", $"Invalid argument '{argument}'.")
	{
		Argument = argument;
	}

	public InvalidArgumentException(string argument, string message) : base("Args", message)
	{
		Argument = argument;
	}
}

public sealed class InvalidSettingException : UtilsException
{
	public string SettingName { get; init; }

	public InvalidSettingException(string settingName) : base("Settings", $"Invalid setting found ({settingName}).")
	{
		SettingName = settingName;
	}

	public InvalidSettingException(string settingName, string message) : base("Settings", message)
	{
		SettingName = settingName;
	}
}
