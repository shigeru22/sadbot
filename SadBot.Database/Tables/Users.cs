// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

using System.Data;
using Npgsql;
using SadBot.Utils;

namespace SadBot.Database.Tables;

public struct UsersTableData
{
	public int UserID { get; set; }
	public string DiscordID { get; set; }
	public string Username { get; set; }
	public string Discriminator { get; set; }
	public DateTime CreationDate { get; set; } // save as UTC
	public DateTime LastUpdate { get; set; } // save as UTC
}

public static class Users
{
	public static async Task<UsersTableData[]> GetAllUsersAsync(DatabaseTransaction transaction)
	{
		const string query = @"
			SELECT
				users.""user_id"",
				users.""discord_id"",
				users.""username"",
				users.""discriminator"",
				users.""creation_date"",
				users.""last_update""
			FROM
				users
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction);
		await using NpgsqlDataReader dbReader = await dbCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

		if (!dbReader.HasRows)
		{
			Log.WriteVerbose("users: Returned 0 rows.");
			return Array.Empty<UsersTableData>();
		}

		List<UsersTableData> ret = new List<UsersTableData>();
		while (await dbReader.ReadAsync())
		{
			ret.Add(GetRowData(dbReader));
		}

		Log.WriteVerbose($"users: Returned {ret.Count} row{(ret.Count == 1 ? string.Empty : "s")}.");
		return ret.ToArray();
	}

	public static async Task<UsersTableData?> GetAllUsersByUserIDAsync(DatabaseTransaction transaction, int dbUserId)
	{
		const string query = @"
			SELECT
				users.""user_id"",
				users.""discord_id"",
				users.""username"",
				users.""discriminator"",
				users.""creation_date"",
				users.""last_update""
			FROM
				users
			WHERE
				users.""user_id"" = ($1)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = dbUserId }
			}
		};
		await using NpgsqlDataReader dbReader = await dbCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

		if (!dbReader.HasRows)
		{
			Log.WriteWarning($"users: User with user_id = {dbUserId} not found. Returning as null.");
			return null;
		}

		if (dbReader.Rows > 1)
		{
			throw new DuplicateRecordException("users", "user_id", dbUserId.ToString(), dbReader.Rows);
		}

		_ = await dbReader.ReadAsync();

		UsersTableData ret = GetRowData(dbReader);

		Log.WriteVerbose("users: Returned 1 row.");
		return ret;
	}

	public static async Task<UsersTableData?> GetAllUsersByDiscordIDAsync(DatabaseTransaction transaction, int dbUserDiscordId)
	{
		const string query = @"
			SELECT
				users.""user_id"",
				users.""discord_id"",
				users.""username"",
				users.""discriminator"",
				users.""creation_date"",
				users.""last_update""
			FROM
				users
			WHERE
				users.""discord_id"" = ($1)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = dbUserDiscordId }
			}
		};
		await using NpgsqlDataReader dbReader = await dbCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

		if (!dbReader.HasRows)
		{
			Log.WriteWarning($"users: User with discord_id = {dbUserDiscordId} not found. Returning as null.");
			return null;
		}

		if (dbReader.Rows > 1)
		{
			throw new DuplicateRecordException("users", "discord_id", dbUserDiscordId.ToString(), dbReader.Rows);
		}

		_ = await dbReader.ReadAsync();

		UsersTableData ret = GetRowData(dbReader);

		Log.WriteVerbose("users: Returned 1 row.");
		return ret;
	}

	public static async Task InsertUserAsync(DatabaseTransaction transaction, string userDiscordId, string username, string discriminator)
	{
		const string query = @"
			INSERT INTO	users (discord_id, username, discriminator)
				VALUES ($1, $2, $3)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = userDiscordId },
				new NpgsqlParameter() { Value = username },
				new NpgsqlParameter() { Value = discriminator }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (discord_id = {userDiscordId}, username = {username}, discriminator = {discriminator}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("users: Inserted 1 row.");
	}

	public static async Task InsertUserAsync(DatabaseTransaction transaction, string userDiscordId, string username, string discriminator, int userId)
	{
		const string query = @"
			INSERT INTO	users (user_id, discord_id, username, discrimintator)
				VALUES ($1, $2, $3)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = userId },
				new NpgsqlParameter() { Value = userDiscordId },
				new NpgsqlParameter() { Value = username },
				new NpgsqlParameter() { Value = discriminator }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (user_id = {userId}, discord_id = {userDiscordId}, username = {username}, discriminator = {discriminator}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("users: Inserted 1 row.");
	}

	public static async Task InsertUserAsync(DatabaseTransaction transaction, string userDiscordId, string username, string discriminator, DateTime lastUpdate, DateTime creationDate)
	{
		const string query = @"
			INSERT INTO	users (discord_id, username, discrimintator, creation_date, last_update)
				VALUES ($1, $2, $3, $4, $5)
		";

		DateTime tempUtcCreationDate = creationDate.ToUniversalTime();
		DateTime tempUtcLastUpdate = lastUpdate.ToUniversalTime();

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = userDiscordId },
				new NpgsqlParameter() { Value = username },
				new NpgsqlParameter() { Value = discriminator },
				new NpgsqlParameter() { Value = tempUtcCreationDate },
				new NpgsqlParameter() { Value = tempUtcLastUpdate }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (discord_id = {userDiscordId}, username = {username}, discriminator = {discriminator}, lastUpdate = {tempUtcLastUpdate}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("users: Inserted 1 row.");
	}

	public static async Task InsertUserAsync(DatabaseTransaction transaction, string userDiscordId, string username, string discriminator, DateTime lastUpdate, DateTime creationDate, int userId)
	{
		const string query = @"
			INSERT INTO	users (discord_id, username, discrimintator, creation_date, last_update)
				VALUES ($1, $2, $3, $4, $5)
		";

		DateTime tempUtcCreationDate = creationDate.ToUniversalTime();
		DateTime tempUtcLastUpdate = lastUpdate.ToUniversalTime();

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = userId },
				new NpgsqlParameter() { Value = userDiscordId },
				new NpgsqlParameter() { Value = username },
				new NpgsqlParameter() { Value = discriminator },
				new NpgsqlParameter() { Value = tempUtcCreationDate },
				new NpgsqlParameter() { Value = tempUtcLastUpdate }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (user_id = {userId}, discord_id = {userDiscordId}, username = {username}, discriminator = {discriminator}, lastUpdate = {tempUtcLastUpdate}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("users: Inserted 1 row.");
	}

	public static async Task UpdateUserAsync(DatabaseTransaction transaction, int userId, string username, string discriminator)
	{
		const string query = @"
			UPDATE users
			SET username = ($1), discriminator = ($2), last_update = (NOW() AT TIME ZONE 'UTC')
			WHERE user_id = ($3)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = username },
				new NpgsqlParameter() { Value = discriminator },
				new NpgsqlParameter() { Value = userId }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Update query failed (username = {username}, discriminator = {discriminator}, user_id = {userId}.");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Update query failed.");
		}

		Log.WriteVerbose("users: Updated 1 row.");
	}

	public static async Task UpdateUserAsync(DatabaseTransaction transaction, string userDiscordId, string username, string discriminator)
	{
		const string query = @"
			UPDATE users
			SET username = ($1), discriminator = ($2), last_update = (NOW() AT TIME ZONE 'UTC')
			WHERE discord_id = ($3)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = username },
				new NpgsqlParameter() { Value = discriminator },
				new NpgsqlParameter() { Value = userDiscordId }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Update query failed (username = {username}, discriminator = {discriminator}, discord_id = {userDiscordId}.");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Update query failed.");
		}

		Log.WriteVerbose("users: Updated 1 row.");
	}

	public static async Task DeleteUserAsync(DatabaseTransaction transaction, int userId)
	{
		const string query = @"
			DELETE FROM users
			WHERE users.""user_id"" = ($1)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = userId }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Deletion query failed (user_id = {userId}.");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Deletion query failed.");
		}

		Log.WriteVerbose("users: Deleted 1 row.");
	}

	public static async Task DeleteUserAsync(DatabaseTransaction transaction, string userDiscordId)
	{
		const string query = @"
			DELETE FROM users
			WHERE users.""discord_id"" = ($1)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = userDiscordId }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Deletion query failed (discord_id = {userDiscordId}.");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Deletion query failed.");
		}

		Log.WriteVerbose("users: Deleted 1 row.");
	}

	public static async Task CreateUsersTable(DatabaseTransaction transaction)
	{
		const string query = @"
			CREATE TABLE users (
				user_id SERIAL PRIMARY KEY,
				discord_id VARCHAR(255),
				username VARCHAR(255),
				discriminator VARCHAR(4),
				creation_date TIMESTAMP NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
				last_update TIMESTAMP NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC')
			)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction);
		_ = await dbCommand.ExecuteNonQueryAsync();
	}

	private static UsersTableData GetRowData(NpgsqlDataReader dbReader)
	{
		int tempUserID = dbReader.GetInt32(0);
		string tempDiscordID = dbReader.GetString(1);
		string tempUsername = dbReader.GetString(2);
		string tempDiscriminator = dbReader.GetString(3);
		DateTime tempCreationDate = dbReader.GetDateTime(4);
		DateTime tempLastUpdate = dbReader.GetDateTime(5);

		return new UsersTableData()
		{
			UserID = tempUserID,
			DiscordID = tempDiscordID,
			Username = tempUsername,
			Discriminator = tempDiscriminator,
			CreationDate = tempCreationDate,
			LastUpdate = tempLastUpdate
		};
	}
}
