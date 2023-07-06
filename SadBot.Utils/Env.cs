// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

using System.Reflection;

namespace SadBot.Utils;

public class EnvironmentWrapper
{
	private static readonly EnvironmentWrapper instance = new EnvironmentWrapper();

	public static EnvironmentWrapper Instance => instance;

	private const string ENV_PREFIX = "SB_";

	[EnvironmentVariable($"{ENV_PREFIX}BOT_TOKEN")] public string? BotToken { get; init; }
	[EnvironmentVariable($"{ENV_PREFIX}USE_REPLY")] public bool? UseReply { get; init; }
	[EnvironmentVariable($"{ENV_PREFIX}LOG_USE_UTC")] public bool? UseUTC { get; init; }
	[EnvironmentVariable($"{ENV_PREFIX}LOG_SEVERITY")] public int? LogSeverity { get; init; }
	[EnvironmentVariable($"{ENV_PREFIX}DB_HOST")] public string? DatabaseHost { get; init; }
	[EnvironmentVariable($"{ENV_PREFIX}DB_PORT")] public int? DatabasePort { get; init; }
	[EnvironmentVariable($"{ENV_PREFIX}DB_USERNAME")] public string? DatabaseUsername { get; init; }
	[EnvironmentVariable($"{ENV_PREFIX}DB_PASSWORD")] public string? DatabasePassword { get; init; }
	[EnvironmentVariable($"{ENV_PREFIX}DB_NAME")] public string? DatabaseName { get; init; }
	[EnvironmentVariable($"{ENV_PREFIX}DB_CAPATH")] public string? DatabaseCertificatePath { get; init; }
	[EnvironmentVariable($"{ENV_PREFIX}DB_USESSL")] public string? DatabaseUseSSL { get; init; }

	private EnvironmentWrapper()
	{
		Log.WriteVerbose("Environment wrapper instance created.");
		PopulateEnvironmentVariables();
	}

	private void PopulateEnvironmentVariables()
	{
		Log.WriteVerbose("Populating environment variables to current environment instance.");

		// get all properties and fields with EnvironmentVariable attribute
		PropertyInfo[] props = GetType()
			.GetProperties()
			.Where(prop => prop.IsDefined(typeof(EnvironmentVariableAttribute), false))
			.ToArray();
		FieldInfo[] fields = GetType()
			.GetFields()
			.Where(prop => prop.IsDefined(typeof(EnvironmentVariableAttribute), false))
			.ToArray();

		// iterate each property and field, get its attribute and set environment value to it
		foreach (PropertyInfo prop in props)
		{
			EnvironmentVariableAttribute? attrProp = prop.GetCustomAttribute<EnvironmentVariableAttribute>();
			if (attrProp != null)
			{
				string? tempEnv = Environment.GetEnvironmentVariable(attrProp.EnvironmentKey);
				object? tempValue = Parser.DynamicParse(tempEnv, prop.PropertyType);
				Log.WriteDebug($"Setting {prop.Name} ({attrProp.EnvironmentKey}) to '{tempValue ?? "null"}'.");
				prop.SetValue(this, tempValue);
			}
		}
		foreach (FieldInfo field in fields)
		{
			EnvironmentVariableAttribute? attrField = field.GetCustomAttribute<EnvironmentVariableAttribute>();
			if (attrField != null)
			{
				string? tempEnv = Environment.GetEnvironmentVariable(attrField.EnvironmentKey);
				object? tempValue = Parser.DynamicParse(tempEnv, field.FieldType);
				Log.WriteDebug($"Setting {field.Name} ({attrField.EnvironmentKey}) to '{tempValue ?? "null"}'.");
				field.SetValue(this, tempValue);
			}
		}
	}
}
