// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

namespace SadBot.Utils;

public static class Parser
{
	public static object? DynamicParse(string? value, Type targetType)
	{
		// special case: if type is boolean
		if (targetType == typeof(bool?))
		{
			return value != null && (value.Equals("1") || value.ToLower().Equals("true"));
		}

		Type undTargetType = Nullable.GetUnderlyingType(targetType) ?? targetType;
		return value == null ? null : Convert.ChangeType(value, undTargetType);
	}

	public static T? DynamicParse<T>(string? value)
	{
		Type tempTargetType = typeof(T);

		// special case: if type is boolean
		if (tempTargetType == typeof(bool))
		{
			return (T)(object)(value != null && (value.Equals("1") || value.ToLower().Equals("true")));
		}

		Type undTargetType = Nullable.GetUnderlyingType(tempTargetType) ?? tempTargetType;
		return (T?)(value == null ? null : Convert.ChangeType(value, undTargetType));
	}
}
