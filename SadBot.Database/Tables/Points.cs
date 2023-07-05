// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

using System.Data;
using Npgsql;
using SadBot.Utils;

namespace SadBot.Database.Tables;

public struct PointsTableData
{
	public int PointID { get; set; }
	public UsersTableData User { get; set; }
	public GuildsTableData Guild { get; set; }
	public DifficultiesTableData Difficulty { get; set; }
	public int Points { get; set; }
	public DateTime CreationDate { get; set; } // set as UTC
	public DateTime LastUpdate { get; set; } // set as UTC
}

public static class Points
{
	public static async Task<PointsTableData[]> GetAllPointsAsync(DatabaseTransaction transaction)
	{
		const string query = @"
			SELECT
				points.""point_id"",
				users.""user_id"",
				users.""discord_id"",
				users.""username"",
				users.""discriminator"",
				users.""creation_date"" AS user_creation_date,
				users.""last_update"" AS user_last_update,
				guilds.""guild_id"",
				guilds.""discord_id"",
				guilds.""guild_name"",
				guilds.""creation_date"" AS guild_creation_date,
				guilds.""last_update"" AS guild_last_update,
				diffs.""diff_id"",
				diffs.""diff_name"",
				diffs.""diff_short_name"",
				diffs.""color"",
				diffs.""creation_date"" AS diff_creation_date,
				diffs.""last_update"" AS diff_last_update,
				points.""points"",
				points.""creation_date"" AS point_creation_date,
				points.""last_update"" AS point_last_update
			FROM
				points
			JOIN
				users ON users.""user_id"" = points.""user_id""
			JOIN
				guilds ON guilds.""guild_id"" = points.""guild_id""
			JOIN
				diffs ON diffs.""diff_id"" = points.""diff_id""
			ORDER BY points.""point_id""
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction);
		await using NpgsqlDataReader dbReader = await dbCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

		if (!dbReader.HasRows)
		{
			Log.WriteVerbose("points: Returned 0 rows.");
			return Array.Empty<PointsTableData>();
		}

		List<PointsTableData> ret = new List<PointsTableData>();
		while (await dbReader.ReadAsync())
		{
			ret.Add(GetRowData(dbReader));
		}

		Log.WriteVerbose($"points: Returned {ret.Count} row{(ret.Count == 1 ? string.Empty : "s")}.");
		return ret.ToArray();
	}

	public static async Task<PointsTableData[]> GetAllPointsByUserIDAsync(DatabaseTransaction transaction, int userId)
	{
		const string query = @"
			SELECT
				points.""point_id"",
				users.""user_id"",
				users.""discord_id"",
				users.""username"",
				users.""discriminator"",
				users.""creation_date"" AS user_creation_date,
				users.""last_update"" AS user_last_update,
				guilds.""guild_id"",
				guilds.""discord_id"",
				guilds.""guild_name"",
				guilds.""creation_date"" AS guild_creation_date,
				guilds.""last_update"" AS guild_last_update,
				diffs.""diff_id"",
				diffs.""diff_name"",
				diffs.""diff_short_name"",
				diffs.""color"",
				diffs.""creation_date"" AS diff_creation_date,
				diffs.""last_update"" AS diff_last_update,
				points.""points"",
				points.""creation_date"" AS point_creation_date,
				points.""last_update"" AS point_last_update
			FROM
				points
			JOIN
				users ON users.""user_id"" = points.""user_id""
			JOIN
				guilds ON guilds.""guild_id"" = points.""guild_id""
			JOIN
				diffs ON diffs.""diff_id"" = points.""diff_id""
			WHERE
				points.""user_id"" = ($1)
			ORDER BY points.""point_id""
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = userId }
			}
		};
		await using NpgsqlDataReader dbReader = await dbCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

		if (!dbReader.HasRows)
		{
			Log.WriteVerbose("points: Returned 0 rows.");
			return Array.Empty<PointsTableData>();
		}

		List<PointsTableData> ret = new List<PointsTableData>();
		while (await dbReader.ReadAsync())
		{
			ret.Add(GetRowData(dbReader));
		}

		Log.WriteVerbose($"points: Returned {ret.Count} row{(ret.Count == 1 ? string.Empty : "s")}.");
		return ret.ToArray();
	}

	public static async Task<PointsTableData[]> GetAllPointsByUserIDAsync(DatabaseTransaction transaction, int userId, int guildId)
	{
		const string query = @"
			SELECT
				points.""point_id"",
				users.""user_id"",
				users.""discord_id"",
				users.""username"",
				users.""discriminator"",
				users.""creation_date"" AS user_creation_date,
				users.""last_update"" AS user_last_update,
				guilds.""guild_id"",
				guilds.""discord_id"",
				guilds.""guild_name"",
				guilds.""creation_date"" AS guild_creation_date,
				guilds.""last_update"" AS guild_last_update,
				diffs.""diff_id"",
				diffs.""diff_name"",
				diffs.""diff_short_name"",
				diffs.""color"",
				diffs.""creation_date"" AS diff_creation_date,
				diffs.""last_update"" AS diff_last_update,
				points.""points"",
				points.""creation_date"" AS point_creation_date,
				points.""last_update"" AS point_last_update
			FROM
				points
			JOIN
				users ON users.""user_id"" = points.""user_id""
			JOIN
				guilds ON guilds.""guild_id"" = points.""guild_id""
			JOIN
				diffs ON diffs.""diff_id"" = points.""diff_id""
			WHERE
				points.""user_id"" = ($1) AND points.""guild_id"" = ($2)
			ORDER BY points.""point_id""
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = userId },
				new NpgsqlParameter() { Value = guildId }
			}
		};
		await using NpgsqlDataReader dbReader = await dbCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

		if (!dbReader.HasRows)
		{
			Log.WriteVerbose("points: Returned 0 rows.");
			return Array.Empty<PointsTableData>();
		}

		List<PointsTableData> ret = new List<PointsTableData>();
		while (await dbReader.ReadAsync())
		{
			ret.Add(GetRowData(dbReader));
		}

		Log.WriteVerbose($"points: Returned {ret.Count} row{(ret.Count == 1 ? string.Empty : "s")}.");
		return ret.ToArray();
	}

	public static async Task<PointsTableData[]> GetAllPointsByGuildIDAsync(DatabaseTransaction transaction, int guildId)
	{
		const string query = @"
			SELECT
				points.""point_id"",
				users.""user_id"",
				users.""discord_id"",
				users.""username"",
				users.""discriminator"",
				users.""creation_date"" AS user_creation_date,
				users.""last_update"" AS user_last_update,
				guilds.""guild_id"",
				guilds.""discord_id"",
				guilds.""guild_name"",
				guilds.""creation_date"" AS guild_creation_date,
				guilds.""last_update"" AS guild_last_update,
				diffs.""diff_id"",
				diffs.""diff_name"",
				diffs.""diff_short_name"",
				diffs.""color"",
				diffs.""creation_date"" AS diff_creation_date,
				diffs.""last_update"" AS diff_last_update,
				points.""points"",
				points.""creation_date"" AS point_creation_date,
				points.""last_update"" AS point_last_update
			FROM
				points
			JOIN
				users ON users.""user_id"" = points.""user_id""
			JOIN
				guilds ON guilds.""guild_id"" = points.""guild_id""
			JOIN
				diffs ON diffs.""diff_id"" = points.""diff_id""
			WHERE
				points.""guild_id"" = ($1)
			ORDER BY points.""point_id""
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = guildId }
			}
		};
		await using NpgsqlDataReader dbReader = await dbCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

		if (!dbReader.HasRows)
		{
			Log.WriteVerbose("points: Returned 0 rows.");
			return Array.Empty<PointsTableData>();
		}

		List<PointsTableData> ret = new List<PointsTableData>();
		while (await dbReader.ReadAsync())
		{
			ret.Add(GetRowData(dbReader));
		}

		Log.WriteVerbose($"points: Returned {ret.Count} row{(ret.Count == 1 ? string.Empty : "s")}.");
		return ret.ToArray();
	}

	public static async Task InsertPointAsync(DatabaseTransaction transaction, int userId, int guildId, int diffId)
	{
		const string query = @"
			INSERT INTO points (user_id, guild_id, diff_id)
				VALUES ($1, $2, $3)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = userId },
				new NpgsqlParameter() { Value = guildId },
				new NpgsqlParameter() { Value = diffId }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (user_id = {userId}, guild_id = {guildId}, diff_id = {diffId}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("points: Inserted 1 row.");
	}

	public static async Task InsertPointAsync(DatabaseTransaction transaction, int userId, int guildId, int diffId, int pointId)
	{
		const string query = @"
			INSERT INTO points (pointId, user_id, guild_id, diff_id)
				VALUES ($1, $2, $3, $4)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = pointId },
				new NpgsqlParameter() { Value = userId },
				new NpgsqlParameter() { Value = guildId },
				new NpgsqlParameter() { Value = diffId }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (point_id = {pointId}, user_id = {userId}, guild_id = {guildId}, diff_id = {diffId}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("points: Inserted 1 row.");
	}

	public static async Task InsertPointAsync(DatabaseTransaction transaction, int userId, int guildId, int diffId, DateTime creationDate, DateTime lastUpdate)
	{
		const string query = @"
			INSERT INTO points (user_id, guild_id, diff_id, creation_date, last_update)
				VALUES ($1, $2, $3, $4, $5)
		";

		DateTime tempUtcCreationDate = creationDate.ToUniversalTime();
		DateTime tempUtcLastUpdate = lastUpdate.ToUniversalTime();

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = userId },
				new NpgsqlParameter() { Value = guildId },
				new NpgsqlParameter() { Value = diffId },
				new NpgsqlParameter() { Value = tempUtcCreationDate },
				new NpgsqlParameter() { Value = tempUtcLastUpdate }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (user_id = {userId}, guild_id = {guildId}, diff_id = {diffId}, creation_date = {tempUtcCreationDate}, last_update = {tempUtcLastUpdate}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("points: Inserted 1 row.");
	}

	public static async Task InsertPointAsync(DatabaseTransaction transaction, int userId, int guildId, int diffId, DateTime creationDate, DateTime lastUpdate, int pointId)
	{
		const string query = @"
			INSERT INTO points (point_id, user_id, guild_id, diff_id, creation_date, last_update)
				VALUES ($1, $2, $3, $4, $5, $6)
		";

		DateTime tempUtcCreationDate = creationDate.ToUniversalTime();
		DateTime tempUtcLastUpdate = lastUpdate.ToUniversalTime();

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = pointId },
				new NpgsqlParameter() { Value = userId },
				new NpgsqlParameter() { Value = guildId },
				new NpgsqlParameter() { Value = diffId },
				new NpgsqlParameter() { Value = tempUtcCreationDate },
				new NpgsqlParameter() { Value = tempUtcLastUpdate }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (point_id = {pointId}, user_id = {userId}, guild_id = {guildId}, diff_id = {diffId}, creation_date = {tempUtcCreationDate}, last_update = {tempUtcLastUpdate}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("points: Inserted 1 row.");
	}

	public static async Task UpdatePointAsync(DatabaseTransaction transaction, int pointId, int points)
	{
		const string query = @"
			UPDATE points
			SET points = ($1)
			WHERE point_id = ($2)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = points },
				new NpgsqlParameter() { Value = pointId }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Update query failed (point_id = {pointId} -> points = {points}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Update query failed.");
		}

		Log.WriteVerbose("points: Updated 1 row.");
	}

	public static async Task CreatePointsTable(DatabaseTransaction transaction)
	{
		const string query = @"
			CREATE TABLE points (
				point_id SERIAL PRIMARY KEY,
				user_id INTEGER NOT NULL,
				guild_id INTEGER NOT NULL,
				diff_id INTEGER NOT NULL,
				points INTEGER NOT NULL DEFAULT 0,
				creation_date TIMESTAMP NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
				last_update TIMESTAMP NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC')
			)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction);
		_ = await dbCommand.ExecuteNonQueryAsync();
	}

	private static PointsTableData GetRowData(NpgsqlDataReader dbReader)
	{
		int tempPointId = dbReader.GetInt32(0);
		int tempUserId = dbReader.GetInt32(1);
		string tempUserDiscordId = dbReader.GetString(2);
		string tempUsername = dbReader.GetString(3);
		string tempUserDiscriminator = dbReader.GetString(4);
		DateTime tempUserCreationDate = dbReader.GetDateTime(5);
		DateTime tempUserLastUpdate = dbReader.GetDateTime(6);
		int tempGuildId = dbReader.GetInt32(7);
		string tempGuildDiscordId = dbReader.GetString(8);
		string tempGuildName = dbReader.GetString(9);
		DateTime tempGuildCreationDate = dbReader.GetDateTime(10);
		DateTime tempGuildLastUpdate = dbReader.GetDateTime(11);
		int tempDiffId = dbReader.GetInt32(12);
		string tempDiffName = dbReader.GetString(13);
		string tempDiffShortName = dbReader.GetString(14);
		string tempDiffColor = dbReader.GetString(15);
		DateTime tempDiffCreationDate = dbReader.GetDateTime(16);
		DateTime tempDiffLastUpdate = dbReader.GetDateTime(17);
		int tempPoints = dbReader.GetInt32(18);
		DateTime tempPointCreationDate = dbReader.GetDateTime(19);
		DateTime tempPointLastUpdate = dbReader.GetDateTime(20);

		return new PointsTableData()
		{
			PointID = tempPointId,
			User = new UsersTableData()
			{
				UserID = tempUserId,
				DiscordID = tempUserDiscordId,
				Username = tempUsername,
				Discriminator = tempUserDiscriminator,
				CreationDate = tempUserCreationDate,
				LastUpdate = tempUserLastUpdate
			},
			Guild = new GuildsTableData()
			{
				GuildID = tempGuildId,
				DiscordID = tempGuildDiscordId,
				GuildName = tempGuildName,
				CreationDate = tempGuildCreationDate,
				LastUpdate = tempGuildLastUpdate
			},
			Difficulty = new DifficultiesTableData()
			{
				DiffID = tempDiffId,
				DiffName = tempDiffName,
				DiffShortName = tempDiffShortName,
				Color = $"#{tempDiffColor}",
				CreationDate = tempDiffCreationDate,
				LastUpdate = tempDiffLastUpdate
			},
			Points = tempPoints,
			CreationDate = tempPointCreationDate,
			LastUpdate = tempPointLastUpdate
		};
	}
}
