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

public class DatabaseInstanceException : DatabaseException
{
	public string Query { get; init; }

	public DatabaseInstanceException(string query) : base("Database instance exception occurred while executing query.")
	{
		Query = query;
	}

	public DatabaseInstanceException(string query, string message) : base(message)
	{
		Query = query;
	}
}

public class DuplicateRecordException : DatabaseException
{
	public string TableName { get; init; }
	public string ColumnName { get; init; }
	public string ParameterData { get; init; }
	public ulong DuplicatesFound { get; init; }

	public DuplicateRecordException(string tableName, string columnName, string parameterData, ulong duplicatesFound) : base($"Duplicated data found ({tableName} table, {columnName} = {parameterData}).")
	{
		TableName = tableName;
		ColumnName = columnName;
		ParameterData = parameterData;
		DuplicatesFound = duplicatesFound;
	}

	public DuplicateRecordException(string tableName, string columnName, string parameterData, ulong duplicatesFound, string message) : base(message)
	{
		TableName = tableName;
		ColumnName = columnName;
		ParameterData = parameterData;
		DuplicatesFound = duplicatesFound;
	}
}
