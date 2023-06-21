// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

namespace SadBot.Database;

public class DatabaseException : Exception
{
	public DatabaseException() : base("Database exception occurred.") { }
	public DatabaseException(string message) : base(message) { }
}

public class DatabaseConfigException : DatabaseException
{
	public DatabaseConfigException() : base("Invalid database configuration.") { }
	public DatabaseConfigException(string message) : base(message) { }
}

public class DatabaseParameterException : DatabaseException
{
	public string Parameter { get; init; }

	public DatabaseParameterException(string parameter) : base($"Invalid config parameter ({parameter}).")
	{
		Parameter = parameter;
	}

	public DatabaseParameterException(string parameter, string message) : base(message)
	{
		Parameter = parameter;
	}
}
