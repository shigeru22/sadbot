// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

using System.Reflection;

namespace SadBot.Utils;

public class ArgumentsWrapper
{
	private static readonly ArgumentsWrapper instance = new ArgumentsWrapper();

	public static ArgumentsWrapper Instance => instance;

	[Argument("t", "bot-token", true)] public string? BotToken { get; init; }
	[Argument("r", "use-reply")] public bool? UseReply { get; init; }
	[Argument("u", "use-utc")] public bool? UseUTC { get; init; }
	[Argument("l", "log-severity", true)] public int? LogSeverity { get; init; }
	[Argument("dh", "db-host", true)] public string? DatabaseHost { get; init; }
	[Argument("dp", "db-port", true)] public string? DatabasePort { get; init; }
	[Argument("du", "db-username", true)] public string? DatabaseUsername { get; init; }
	[Argument("dp", "db-password", true)] public string? DatabasePassword { get; init; }
	[Argument("dn", "db-name", true)] public string? DatabaseName { get; init; }
	[Argument("dc", "db-capath", true)] public string? DatabaseCertificatePath { get; init; }
	[Argument("ds", "db-usessl", true)] public string? DatabaseUseSSL { get; init; }

	private ArgumentsWrapper()
	{
		Log.WriteVerbose("Arguments wrapper instance created.");
		PopulateArguments();
	}

	private void PopulateArguments()
	{
		Log.WriteVerbose("Populating command arguments to current arguments instance.");

		string[] args = Environment.GetCommandLineArgs();

		// get all properties and fields with Argument attribute
		PropertyInfo[] props = GetType()
			.GetProperties()
			.Where(prop => prop.IsDefined(typeof(ArgumentAttribute), false))
			.ToArray();
		FieldInfo[] fields = GetType()
			.GetFields()
			.Where(prop => prop.IsDefined(typeof(ArgumentAttribute), false))
			.ToArray();

		// iterate each argument (start from index 1) and search for matching flag
		int argsLength = args.Length;
		for (int i = 1; i < argsLength; i++)
		{
			bool isLongFlag = false;
			int idxProp = -1;
			int idxField = -1;

			if (args[i].StartsWith("--"))
			{
				idxProp = Array.FindIndex(props, prop =>
				{
					ArgumentAttribute? attrProp = prop.GetCustomAttribute<ArgumentAttribute>();
					return attrProp != null && !string.IsNullOrWhiteSpace(attrProp.LongFlag) && attrProp.LongFlag.Equals(args[i][2..]);
				});
				idxField = Array.FindIndex(fields, field =>
				{
					ArgumentAttribute? attrField = field.GetCustomAttribute<ArgumentAttribute>();
					return attrField != null && !string.IsNullOrWhiteSpace(attrField.LongFlag) && attrField.LongFlag.Equals(args[i][2..]);
				});

				isLongFlag = true;
			}
			else if (args[i].StartsWith("-"))
			{
				idxProp = Array.FindIndex(props, prop =>
				{
					ArgumentAttribute? attrProp = prop.GetCustomAttribute<ArgumentAttribute>();
					return attrProp != null && !string.IsNullOrWhiteSpace(attrProp.ShortFlag) && attrProp.ShortFlag.Equals(args[i][1..]);
				});
				idxField = Array.FindIndex(fields, field =>
				{
					ArgumentAttribute? attrField = field.GetCustomAttribute<ArgumentAttribute>();
					return attrField != null && !string.IsNullOrWhiteSpace(attrField.ShortFlag) && attrField.ShortFlag.Equals(args[i][1..]);
				});
			}
			else
			{
				throw new InvalidArgumentException(args[i]);
			}

			// either prop or field shoule be > 1, not both
			if (idxProp >= 0 && idxField >= 0)
			{
				throw new InvalidProgramException($"There should be not more than 1 field or properties having the same argument flag ('{args[i]}').");
			}

			// set value based on next argument value (if exists)
			if (idxProp >= 0)
			{
				ArgumentAttribute? attrProp = props[idxProp].GetCustomAttribute<ArgumentAttribute>();
				if (attrProp != null)
				{
					if (!attrProp.HasValue && !(props[idxProp].PropertyType == typeof(bool?) || props[idxProp].PropertyType == typeof(bool)))
					{
						// arguments without any value must be used with boolean
						throw new InvalidProgramException($"Argument without any value must be used with boolean type ({args[i]}).");
					}

					if (attrProp.HasValue && i == argsLength - 1)
					{
						// argument value must be set (if last)
						throw new InvalidArgumentException($"Argument value must be provided ({args[i]}).");
					}

					if (attrProp.HasValue)
					{
						object? tempValue = Parser.DynamicParse(args[i + 1], props[idxProp].PropertyType);
						Log.WriteDebug($"Setting {props[idxProp].Name} ({(isLongFlag ? $"--{attrProp.LongFlag}" : $"-{attrProp.ShortFlag}")}) to '{tempValue ?? "null"}'.");
						props[idxProp].SetValue(this, tempValue);

						i++; // increment i if value is found
					}
					else if (props[idxProp].PropertyType == typeof(bool?) || props[idxProp].PropertyType == typeof(bool))
					{
						Log.WriteDebug($"Setting {props[idxProp].Name} ({(isLongFlag ? $"--{attrProp.LongFlag}" : $"-{attrProp.ShortFlag}")}) to 'true'.");
						props[idxProp].SetValue(this, true);
					}
				}
			}
			else if (idxField >= 0)
			{
				ArgumentAttribute? attrField = fields[idxField].GetCustomAttribute<ArgumentAttribute>();
				if (attrField != null)
				{
					if (!attrField.HasValue && !(fields[idxField].FieldType == typeof(bool?) || fields[idxField].FieldType == typeof(bool)))
					{
						// arguments without any value must be used with boolean
						throw new InvalidProgramException($"Argument without any value must be used with boolean type ({args[i]}).");
					}

					if (attrField.HasValue && i == argsLength - 1)
					{
						// argument value must be set (if last)
						throw new InvalidArgumentException($"Argument value must be provided ({args[i]}).");
					}

					if (attrField.HasValue)
					{
						object? tempValue = Parser.DynamicParse(args[i + 1], fields[idxField].FieldType);
						Log.WriteDebug($"Setting {fields[idxField].Name} ({(isLongFlag ? $"--{attrField.LongFlag}" : $"-{attrField.ShortFlag}")}) to '{tempValue ?? "null"}'.");
						fields[idxField].SetValue(this, tempValue);

						i++; // increment i if value is found
					}
					else if (fields[idxField].FieldType == typeof(bool?) || fields[idxField].FieldType == typeof(bool))
					{
						Log.WriteDebug($"Setting {fields[idxField].Name} ({(isLongFlag ? $"--{attrField.LongFlag}" : $"-{attrField.ShortFlag}")}) to 'true'.");
						fields[idxField].SetValue(this, true);
					}
				}
			}
		}
	}
}
