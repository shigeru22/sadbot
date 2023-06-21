// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

using Npgsql;
using SadBot.Utils;

namespace SadBot.Database;

public class DatabaseFactory
{
	private static readonly DatabaseFactory instance = new DatabaseFactory();

	public static DatabaseFactory Instance => instance;

	private NpgsqlDataSource? dataSource = null;

	private DatabaseFactory()
	{
		Log.WriteVerbose("DatabaseFactory instance created.");
	}

	public void Configure(DatabaseConfig config)
	{
		dataSource = NpgsqlDataSource.Create(config.ToConnectionString());
	}

	public void Configure(string connectionString)
	{
		dataSource = NpgsqlDataSource.Create(connectionString);
	}

	public async Task<DatabaseTransaction> CreateTransaction()
	{
		if (dataSource == null)
		{
			throw new DatabaseException("Factory configuration hasn't been set. Use Configure() first.");
		}

		NpgsqlConnection conn = await dataSource.OpenConnectionAsync();
		return new DatabaseTransaction(conn);
	}
}
