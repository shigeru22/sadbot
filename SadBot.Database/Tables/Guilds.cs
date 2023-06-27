// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

using System.Data;
using Npgsql;
using SadBot.Utils;

namespace SadBot.Database.Tables;

public struct GuildsTableData
{
	public int GuildID { get; set; }
	public string DiscordID { get; set; }
	public string GuildName { get; set; }
	public DateTime CreationDate { get; set; } // set as UTC
	public DateTime LastUpdate { get; set; } // set as UTC
}

public static class Guilds
{
	public static async Task<GuildsTableData[]> GetAllGuildsAsync(DatabaseTransaction transaction)
	{
		const string query = @"
			SELECT
				guilds.""guild_id"",
				guilds.""discord_id"",
				guilds.""guild_name"",
				guilds.""creation_date"",
				guilds.""last_update""
			FROM
				guilds
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction);
		await using NpgsqlDataReader dbReader = await dbCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

		if (!dbReader.HasRows)
		{
			Log.WriteVerbose("guilds: Returned 0 rows.");
			return Array.Empty<GuildsTableData>();
		}

		List<GuildsTableData> ret = new List<GuildsTableData>();
		while (await dbReader.ReadAsync())
		{
			ret.Add(new GuildsTableData()
			{
				GuildID = dbReader.GetInt32(0),
				DiscordID = dbReader.GetString(1),
				GuildName = dbReader.GetString(2),
				CreationDate = dbReader.GetDateTime(3),
				LastUpdate = dbReader.GetDateTime(4)
			});
		}

		Log.WriteVerbose($"guilds: Returned {ret.Count} row{(ret.Count == 1 ? string.Empty : "s")}.");
		return ret.ToArray();
	}

	public static async Task<GuildsTableData?> GetGuildsByGuildIDAsync(DatabaseTransaction transaction, int dbGuildId)
	{
		const string query = @"
			SELECT
				guilds.""guild_id"",
				guilds.""discord_id"",
				guilds.""guild_name"",
				guilds.""creation_date"",
				guilds.""last_update""
			FROM
				guilds
			WHERE
				guilds.""guild_id"" = ($1)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = dbGuildId }
			}
		};
		await using NpgsqlDataReader dbReader = await dbCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

		if (!dbReader.HasRows)
		{
			Log.WriteWarning($"guilds: Guild with guild_id = {dbGuildId} not found. Returning as null.");
			return null;
		}

		if (dbReader.Rows > 1)
		{
			throw new DuplicateRecordException("guild", "guild_id", dbGuildId.ToString(), dbReader.Rows);
		}

		_ = await dbReader.ReadAsync();

		GuildsTableData ret = new GuildsTableData()
		{
			GuildID = dbReader.GetInt32(0),
			DiscordID = dbReader.GetString(1),
			GuildName = dbReader.GetString(2),
			CreationDate = dbReader.GetDateTime(3),
			LastUpdate = dbReader.GetDateTime(4)
		};

		Log.WriteVerbose("guilds: Returned 1 row.");
		return ret;
	}

	public static async Task<GuildsTableData?> GetGuildsByDiscordIDAsync(DatabaseTransaction transaction, string dbGuildDiscordId)
	{
		const string query = @"
			SELECT
				guilds.""guild_id"",
				guilds.""discord_id"",
				guilds.""guild_name"",
				guilds.""creation_date"",
				guilds.""last_update""
			FROM
				guilds
			WHERE
				guilds.""discord_id"" = ($1)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = dbGuildDiscordId }
			}
		};
		await using NpgsqlDataReader dbReader = await dbCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

		if (!dbReader.HasRows)
		{
			Log.WriteWarning($"guilds: Guild with discord_id = {dbGuildDiscordId} not found. Returning as null.");
			return null;
		}

		if (dbReader.Rows > 1)
		{
			throw new DuplicateRecordException("guild", "discord_id", dbGuildDiscordId.ToString(), dbReader.Rows);
		}

		_ = await dbReader.ReadAsync();

		GuildsTableData ret = new GuildsTableData()
		{
			GuildID = dbReader.GetInt32(0),
			DiscordID = dbReader.GetString(1),
			GuildName = dbReader.GetString(2),
			CreationDate = dbReader.GetDateTime(3),
			LastUpdate = dbReader.GetDateTime(4)
		};

		Log.WriteVerbose("guilds: Returned 1 row.");
		return ret;
	}

	public static async Task InsertGuildAsync(DatabaseTransaction transaction, string guildDiscordId, string guildName)
	{
		const string query = @"
			INSERT INTO guilds (discord_id, guild_name)
				VALUES ($1, $2)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = guildDiscordId },
				new NpgsqlParameter() { Value = guildName }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (discord_id = {guildDiscordId}, guild_name = {guildName}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("guilds: Inserted 1 row.");
	}

	public static async Task InsertGuildAsync(DatabaseTransaction transaction, string guildDiscordId, string guildName, int guildId)
	{
		const string query = @"
			INSERT INTO guilds (guild_id, discord_id, guild_name)
				VALUES ($1, $2, $3)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = guildId },
				new NpgsqlParameter() { Value = guildDiscordId },
				new NpgsqlParameter() { Value = guildName }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (discord_id = {guildDiscordId}, guild_name = {guildName}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("guilds: Inserted 1 row.");
	}

	public static async Task InsertGuildAsync(DatabaseTransaction transaction, string guildDiscordId, string guildName, DateTime creationDate, DateTime lastUpdate)
	{
		const string query = @"
			INSERT INTO guilds (discord_id, guild_name, creation_date, last_update)
				VALUES ($1, $2, $3, $4)
		";

		DateTime tempUtcCreationDate = creationDate.ToUniversalTime();
		DateTime tempUtcLastUpdate = lastUpdate.ToUniversalTime();

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = guildDiscordId },
				new NpgsqlParameter() { Value = guildName },
				new NpgsqlParameter() { Value = tempUtcCreationDate },
				new NpgsqlParameter() { Value = tempUtcLastUpdate }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (discord_id = {guildDiscordId}, guild_name = {guildName}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("guilds: Inserted 1 row.");
	}

	public static async Task InsertGuildAsync(DatabaseTransaction transaction, string guildDiscordId, string guildName, DateTime creationDate, DateTime lastUpdate, int guildId)
	{
		const string query = @"
			INSERT INTO guilds (guild_id, discord_id, guild_name, creation_date, last_update)
				VALUES ($1, $2, $3, $4, $5)
		";

		DateTime tempUtcCreationDate = creationDate.ToUniversalTime();
		DateTime tempUtcLastUpdate = lastUpdate.ToUniversalTime();

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = guildId },
				new NpgsqlParameter() { Value = guildDiscordId },
				new NpgsqlParameter() { Value = guildName },
				new NpgsqlParameter() { Value = tempUtcCreationDate },
				new NpgsqlParameter() { Value = tempUtcLastUpdate }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (discord_id = {guildDiscordId}, guild_name = {guildName}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("guilds: Inserted 1 row.");
	}

	public static async Task UpdateGuildAsync(DatabaseTransaction transaction, int guildId, string guildName)
	{
		const string query = @"
			UPDATE guilds
			SET guild_name = ($1), last_update = (NOW() AT TIME ZONE 'UTC')
			WHERE guild_id = ($2)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = guildName },
				new NpgsqlParameter() { Value = guildId }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Update query failed (guild_name = {guildName}, guild_id = {guildId}.");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Update query failed.");
		}

		Log.WriteVerbose("guilds: Updated 1 row.");
	}

	public static async Task UpdateGuildAsync(DatabaseTransaction transaction, string guildDiscordId, string guildName)
	{
		const string query = @"
			UPDATE guilds
			SET guild_name = ($1), last_update = (NOW() AT TIME ZONE 'UTC')
			WHERE discord_id = ($2)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = guildName },
				new NpgsqlParameter() { Value = guildDiscordId }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Update query failed (guild_name = {guildName}, discord_id = {guildDiscordId}.");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Update query failed.");
		}

		Log.WriteVerbose("guilds: Updated 1 row.");
	}

	public static async Task DeleteGuildAsync(DatabaseTransaction transaction, int guildId)
	{
		const string query = @"
			DELETE FROM guilds
			WHERE guilds.""guild_id"" = ($1)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = guildId }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Deletion query failed (guild_id = {guildId})");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Deletion query failed.");
		}

		Log.WriteVerbose("guilds: Deleted 1 row.");
	}

	public static async Task DeleteGuildAsync(DatabaseTransaction transaction, string guildDiscordId)
	{
		const string query = @"
			DELETE FROM guilds
			WHERE guilds.""discord_id"" = ($1)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = guildDiscordId }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Deletion query failed (discord_id = {guildDiscordId})");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Deletion query failed.");
		}

		Log.WriteVerbose("guilds: Deleted 1 row.");
	}

	public static async Task CreateGuildsTable(DatabaseTransaction transaction)
	{
		const string query = @"
			CREATE TABLE guilds (
				guild_id SERIAL PRIMARY KEY,
				discord_id VARCHAR(255),
				guild_name VARCHAR(255),
				creation_date TIMESTAMP NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
				last_update TIMESTAMP NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC')
			)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction);
		_ = await dbCommand.ExecuteNonQueryAsync();
	}
}
