// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

namespace SadBot.Utils;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class EnvironmentVariableAttribute : Attribute
{
	public string EnvironmentKey { get; init; }

	public EnvironmentVariableAttribute(string environmentKey)
	{
		EnvironmentKey = environmentKey;
	}
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ArgumentAttribute : Attribute
{
	public string? ShortFlag { get; init; }
	public string? LongFlag { get; init; }
	public bool HasValue { get; init; }

	public ArgumentAttribute(string shortFlag, bool hasValue = false)
	{
		ShortFlag = shortFlag;
		HasValue = hasValue;
	}

	public ArgumentAttribute(string? shortFlag, string longFlag, bool hasValue = false)
	{
		ShortFlag = shortFlag;
		LongFlag = longFlag;
		HasValue = hasValue;
	}
}
