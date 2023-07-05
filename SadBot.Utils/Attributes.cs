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
