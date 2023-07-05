// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

using System.Data;
using Npgsql;
using SadBot.Utils;

namespace SadBot.Database.Tables;

public struct ChartsTableData
{
	public int ChartID { get; set; }
	public GuildsTableData Guild { get; set; }
	public DifficultiesTableData Difficulty { get; set; }
	public MaimaiVersionsTableData Version { get; set; }
	public string Artist { get; set; }
	public string Title { get; set; }
	public string AlbumArtURL { get; set; }
	public int Level { get; set; }
	public bool IsLevelPlus { get; set; }
	public decimal LevelPrecise { get; set; }
	public DateTime CreationDate { get; set; }
	public DateTime LastUpdate { get; set; }
}

public static class Charts
{
	public static async Task<ChartsTableData[]> GetAllChartsAsync(DatabaseTransaction transaction)
	{
		const string query = @"
			SELECT
				charts.""chart_id"",
				guilds.""guild_id"",
				guilds.""discord_id"" as guild_discord_id,
				guilds.""guild_name"",
				guilds.""creation_date"",
				guilds.""last_update"",
				diffs.""diff_id"",
				diffs.""diff_name"",
				diffs.""diff_short_name"",
				diffs.""color"",
				diffs.""creation_date"",
				diffs.""last_update"",
				maimai_versions.""version_id"",
				maimai_versions.""version_name"",
				maimai_versions.""is_deluxe"",
				maimai_versions.""launch_date"",
				maimai_versions.""launch_date_global"",
				maimai_versions.""creation_date"",
				maimai_versions.""last_update"",
				charts.""artist"",
				charts.""title"",
				charts.""album_art_url"",
				charts.""level"",
				charts.""is_level_plus"",
				charts.""level_precise"",
				charts.""creation_date"",
				charts.""last_update""
			FROM
				charts
			JOIN
				guilds ON guilds.""guild_id"" = charts.""guild_id""
			JOIN
				diffs ON diffs.""diff_id"" = charts.""diff_id""
			JOIN
				maimai_versions ON maimai_versions.""version_id"" = charts.""version_id""
			ORDER BY charts.""chart_id""
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction);
		await using NpgsqlDataReader dbReader = await dbCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

		if (!dbReader.HasRows)
		{
			Log.WriteVerbose("charts: Returned 0 rows.");
			return Array.Empty<ChartsTableData>();
		}

		List<ChartsTableData> ret = new List<ChartsTableData>();
		while (await dbReader.ReadAsync())
		{
			ret.Add(GetRowData(dbReader));
		}

		Log.WriteVerbose($"charts: Returned {ret.Count} row{(ret.Count == 1 ? string.Empty : "s")}.");
		return ret.ToArray();
	}

	public static async Task<ChartsTableData[]> GetAllChartsByGuildIDAsync(DatabaseTransaction transaction, int guildId)
	{
		const string query = @"
			SELECT
				charts.""chart_id"",
				guilds.""guild_id"",
				guilds.""discord_id"" as guild_discord_id,
				guilds.""guild_name"",
				guilds.""creation_date"",
				guilds.""last_update"",
				diffs.""diff_id"",
				diffs.""diff_name"",
				diffs.""diff_short_name"",
				diffs.""color"",
				diffs.""creation_date"",
				diffs.""last_update"",
				maimai_versions.""version_id"",
				maimai_versions.""version_name"",
				maimai_versions.""is_deluxe"",
				maimai_versions.""launch_date"",
				maimai_versions.""launch_date_global"",
				maimai_versions.""creation_date"",
				maimai_versions.""last_update"",
				charts.""artist"",
				charts.""title"",
				charts.""album_art_url"",
				charts.""level"",
				charts.""is_level_plus"",
				charts.""level_precise"",
				charts.""creation_date"",
				charts.""last_update""
			FROM
				charts
			JOIN
				guilds ON guilds.""guild_id"" = charts.""guild_id""
			JOIN
				diffs ON diffs.""diff_id"" = charts.""diff_id""
			JOIN
				maimai_versions ON maimai_versions.""version_id"" = charts.""version_id""
			WHERE
				charts.""guild_id"" = ($1)
			ORDER BY charts.""chart_id""
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
			Log.WriteVerbose("charts: Returned 0 rows.");
			return Array.Empty<ChartsTableData>();
		}

		List<ChartsTableData> ret = new List<ChartsTableData>();
		while (await dbReader.ReadAsync())
		{
			ret.Add(GetRowData(dbReader));
		}

		Log.WriteVerbose($"charts: Returned {ret.Count} row{(ret.Count == 1 ? string.Empty : "s")}.");
		return ret.ToArray();
	}

	public static async Task<ChartsTableData[]> GetAllChartsByGuildDiscordIDAsync(DatabaseTransaction transaction, string guildDiscordId)
	{
		const string query = @"
			SELECT
				charts.""chart_id"",
				guilds.""guild_id"",
				guilds.""discord_id"" as guild_discord_id,
				guilds.""guild_name"",
				guilds.""creation_date"",
				guilds.""last_update"",
				diffs.""diff_id"",
				diffs.""diff_name"",
				diffs.""diff_short_name"",
				diffs.""color"",
				diffs.""creation_date"",
				diffs.""last_update"",
				maimai_versions.""version_id"",
				maimai_versions.""version_name"",
				maimai_versions.""is_deluxe"",
				maimai_versions.""launch_date"",
				maimai_versions.""launch_date_global"",
				maimai_versions.""creation_date"",
				maimai_versions.""last_update"",
				charts.""artist"",
				charts.""title"",
				charts.""album_art_url"",
				charts.""level"",
				charts.""is_level_plus"",
				charts.""level_precise"",
				charts.""creation_date"",
				charts.""last_update""
			FROM
				charts
			JOIN
				guilds ON guilds.""guild_id"" = charts.""guild_id""
			JOIN
				diffs ON diffs.""diff_id"" = charts.""diff_id""
			JOIN
				maimai_versions ON maimai_versions.""version_id"" = charts.""version_id""
			WHERE
				guilds.""discord_id"" = ($1)
			ORDER BY charts.""chart_id""
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = guildDiscordId }
			}
		};
		await using NpgsqlDataReader dbReader = await dbCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

		if (!dbReader.HasRows)
		{
			Log.WriteVerbose("charts: Returned 0 rows.");
			return Array.Empty<ChartsTableData>();
		}

		List<ChartsTableData> ret = new List<ChartsTableData>();
		while (await dbReader.ReadAsync())
		{
			ret.Add(GetRowData(dbReader));
		}

		Log.WriteVerbose($"charts: Returned {ret.Count} row{(ret.Count == 1 ? string.Empty : "s")}.");
		return ret.ToArray();
	}

	public static async Task<ChartsTableData?> GetChartByChartIDAsync(DatabaseTransaction transaction, int chartId)
	{
		const string query = @"
			SELECT
				charts.""chart_id"",
				guilds.""guild_id"",
				guilds.""discord_id"" as guild_discord_id,
				guilds.""guild_name"",
				guilds.""creation_date"",
				guilds.""last_update"",
				diffs.""diff_id"",
				diffs.""diff_name"",
				diffs.""diff_short_name"",
				diffs.""color"",
				diffs.""creation_date"",
				diffs.""last_update"",
				maimai_versions.""version_id"",
				maimai_versions.""version_name"",
				maimai_versions.""is_deluxe"",
				maimai_versions.""launch_date"",
				maimai_versions.""launch_date_global"",
				maimai_versions.""creation_date"",
				maimai_versions.""last_update"",
				charts.""artist"",
				charts.""title"",
				charts.""album_art_url"",
				charts.""level"",
				charts.""is_level_plus"",
				charts.""level_precise"",
				charts.""creation_date"",
				charts.""last_update""
			FROM
				charts
			JOIN
				guilds ON guilds.""guild_id"" = charts.""guild_id""
			JOIN
				diffs ON diffs.""diff_id"" = charts.""diff_id""
			JOIN
				maimai_versions ON maimai_versions.""version_id"" = charts.""version_id""
			WHERE
				charts.""chart_id"" = ($1)
			ORDER BY charts.""chart_id""
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = chartId }
			}
		};
		await using NpgsqlDataReader dbReader = await dbCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

		if (!dbReader.HasRows)
		{
			Log.WriteWarning($"charts: Chart with chart_id = {chartId} not found. Returning as null.");
			return null;
		}

		if (dbReader.Rows > 1)
		{
			throw new DuplicateRecordException("charts", "chart_id", chartId.ToString(), dbReader.Rows);
		}

		_ = await dbReader.ReadAsync();

		ChartsTableData ret = GetRowData(dbReader);

		Log.WriteVerbose("charts: Returned 1 row.");
		return ret;
	}

	public static async Task InsertChartAsync(DatabaseTransaction transaction, int guildId, int diffId, int versionId, string artist, string title, string albumArtUrl, int level, bool isLevelPlus, decimal levelPrecise)
	{
		const string query = @"
			INSERT INTO charts (guild_id, diff_id, version_id, artist, title, album_art_url, level, is_level_plus, level_precise)
				VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = guildId },
				new NpgsqlParameter() { Value = diffId },
				new NpgsqlParameter() { Value = versionId },
				new NpgsqlParameter() { Value = artist },
				new NpgsqlParameter() { Value = title },
				new NpgsqlParameter() { Value = albumArtUrl },
				new NpgsqlParameter() { Value = level },
				new NpgsqlParameter() { Value = isLevelPlus },
				new NpgsqlParameter() { Value = levelPrecise }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (guild_id = {guildId}, diff_id = {diffId}, version_id = {versionId}, artist = {artist}, title = {title}, album_art_url = {albumArtUrl}, level = {level}, is_level_plus = {isLevelPlus}, level_precise = {levelPrecise}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("charts: Inserted 1 row.");
	}

	public static async Task InsertChartAsync(DatabaseTransaction transaction, int guildId, int diffId, int versionId, string artist, string title, string albumArtUrl, int level, bool isLevelPlus, decimal levelPrecise, int chartId)
	{
		const string query = @"
			INSERT INTO charts (chart_id, guild_id, diff_id, version_id, artist, title, album_art_url, level, is_level_plus, level_precise)
				VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = chartId },
				new NpgsqlParameter() { Value = guildId },
				new NpgsqlParameter() { Value = diffId },
				new NpgsqlParameter() { Value = versionId },
				new NpgsqlParameter() { Value = artist },
				new NpgsqlParameter() { Value = title },
				new NpgsqlParameter() { Value = albumArtUrl },
				new NpgsqlParameter() { Value = level },
				new NpgsqlParameter() { Value = isLevelPlus },
				new NpgsqlParameter() { Value = levelPrecise }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (chart_id = {chartId}, guild_id = {guildId}, diff_id = {diffId}, version_id = {versionId}, artist = {artist}, title = {title}, album_art_url = {albumArtUrl}, level = {level}, is_level_plus = {isLevelPlus}, level_precise = {levelPrecise}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("charts: Inserted 1 row.");
	}

	public static async Task InsertChartAsync(DatabaseTransaction transaction, int guildId, int diffId, int versionId, string artist, string title, string albumArtUrl, int level, bool isLevelPlus, decimal levelPrecise, DateTime creationDate, DateTime lastUpdate)
	{
		const string query = @"
			INSERT INTO charts (guild_id, diff_id, version_id, artist, title, album_art_url, level, is_level_plus, level_precise, creation_date, last_update)
				VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11)
		";

		DateTime tempUtcCreationDate = creationDate.ToUniversalTime();
		DateTime tempUtcLastUpdate = lastUpdate.ToUniversalTime();

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = guildId },
				new NpgsqlParameter() { Value = diffId },
				new NpgsqlParameter() { Value = versionId },
				new NpgsqlParameter() { Value = artist },
				new NpgsqlParameter() { Value = title },
				new NpgsqlParameter() { Value = albumArtUrl },
				new NpgsqlParameter() { Value = level },
				new NpgsqlParameter() { Value = isLevelPlus },
				new NpgsqlParameter() { Value = levelPrecise },
				new NpgsqlParameter() { Value = tempUtcCreationDate },
				new NpgsqlParameter() { Value = tempUtcLastUpdate }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (guild_id = {guildId}, diff_id = {diffId}, version_id = {versionId}, artist = {artist}, title = {title}, album_art_url = {albumArtUrl}, level = {level}, is_level_plus = {isLevelPlus}, level_precise = {levelPrecise}, creation_date = {tempUtcCreationDate}, last_update = {tempUtcLastUpdate}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("charts: Inserted 1 row.");
	}

	public static async Task InsertChartAsync(DatabaseTransaction transaction, int guildId, int diffId, int versionId, string artist, string title, string albumArtUrl, int level, bool isLevelPlus, decimal levelPrecise, DateTime creationDate, DateTime lastUpdate, int chartId)
	{
		const string query = @"
			INSERT INTO charts (chart_id, guild_id, diff_id, version_id, artist, title, album_art_url, level, is_level_plus, level_precise, creation_date, last_update)
				VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12)
		";

		DateTime tempUtcCreationDate = creationDate.ToUniversalTime();
		DateTime tempUtcLastUpdate = lastUpdate.ToUniversalTime();

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = chartId },
				new NpgsqlParameter() { Value = guildId },
				new NpgsqlParameter() { Value = diffId },
				new NpgsqlParameter() { Value = versionId },
				new NpgsqlParameter() { Value = artist },
				new NpgsqlParameter() { Value = title },
				new NpgsqlParameter() { Value = albumArtUrl },
				new NpgsqlParameter() { Value = level },
				new NpgsqlParameter() { Value = isLevelPlus },
				new NpgsqlParameter() { Value = levelPrecise },
				new NpgsqlParameter() { Value = tempUtcCreationDate },
				new NpgsqlParameter() { Value = tempUtcLastUpdate }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (chart_id = {chartId}, guild_id = {guildId}, diff_id = {diffId}, version_id = {versionId}, artist = {artist}, title = {title}, album_art_url = {albumArtUrl}, level = {level}, is_level_plus = {isLevelPlus}, level_precise = {levelPrecise}, creation_date = {tempUtcCreationDate}, last_update = {tempUtcLastUpdate}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("charts: Inserted 1 row.");
	}

	public static async Task UpdateChartAsync(DatabaseTransaction transaction, int chartId, int guildId, int diffId, int versionId, string artist, string title, string albumArtUrl, int level, bool isLevelPlus, bool levelPrecise)
	{
		const string query = @"
			UPDATE charts
			SET guild_id = ($1), diff_id = ($2), version_id = ($3), artist = ($4), title = ($5),
				album_art_url = ($6), level = ($7), is_level_plus = ($8), level_precise = ($9),
				last_update = (NOW() AT TIME ZONE 'UTC')
			WHERE chart_id = ($10)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = guildId },
				new NpgsqlParameter() { Value = diffId },
				new NpgsqlParameter() { Value = versionId },
				new NpgsqlParameter() { Value = artist },
				new NpgsqlParameter() { Value = title },
				new NpgsqlParameter() { Value = albumArtUrl },
				new NpgsqlParameter() { Value = level },
				new NpgsqlParameter() { Value = isLevelPlus },
				new NpgsqlParameter() { Value = levelPrecise },
				new NpgsqlParameter() { Value = chartId }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Update query failed (guild_id = {guildId}, diff_id = {diffId}, version_id = {versionId}, artist = {artist}, title = {title}, album_art_url = {albumArtUrl}, level = {level}, is_level_plus = {isLevelPlus}, level_precise = {levelPrecise}, chart_id = {chartId}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Update query failed.");
		}

		Log.WriteVerbose("charts: Updated 1 row.");
	}

	public static async Task DeleteChartAsync(DatabaseTransaction transaction, int chartId)
	{
		const string query = @"
			DELETE FROM charts
			WHERE chart_id = ($1)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = chartId }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Deletion query failed (chart_id = {chartId})");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Deletion query failed.");
		}

		Log.WriteVerbose("charts: Deleted 1 row.");
	}

	public static async Task CreateChartsTable(DatabaseTransaction transaction)
	{
		const string query = @"
			CREATE TABLE charts (
				chart_id SERIAL PRIMARY KEY,
				guild_id INTEGER NOT NULL,
				diff_id INTEGER NOT NULL,
				version_id INTEGER NOT NULL,
				artist VARCHAR(255) NOT NULL,
				title VARCHAR(255) NOT NULL,
				album_art_url VARCHAR(1024) NOT NULL,
				level INTEGER NOT NULL,
				is_level_plus BOOLEAN NOT NULL,
				level_precise NUMERIC(3, 1) NOT NULL,
				creation_date TIMESTAMP NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
				last_update TIMESTAMP NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
				CONSTRAINT fk_guild
					FOREIGN KEY(guild_id) REFERENCES guilds(guild_id),
				CONSTRAINT fk_diff
					FOREIGN KEY(diff_id) REFERENCES diffs(diff_id),
				CONSTRAINT fk_version
					FOREIGN KEY(version_id) REFERENCES maimai_versions(version_id)
			)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction);
		_ = await dbCommand.ExecuteNonQueryAsync();
	}

	private static ChartsTableData GetRowData(NpgsqlDataReader dbReader)
	{
		int tempChartId = dbReader.GetInt32(0);
		int tempGuildId = dbReader.GetInt32(1);
		string tempGuildDiscordId = dbReader.GetString(2);
		string tempGuildName = dbReader.GetString(3);
		DateTime tempGuildCreationDate = dbReader.GetDateTime(4);
		DateTime tempGuildLastUpdate = dbReader.GetDateTime(5);
		int tempDiffId = dbReader.GetInt32(6);
		string tempDiffName = dbReader.GetString(7);
		string tempDiffShortName = dbReader.GetString(8);
		string tempDiffColor = dbReader.GetString(9);
		DateTime tempDiffCreationDate = dbReader.GetDateTime(10);
		DateTime tempDiffLastUpdate = dbReader.GetDateTime(11);
		int tempMaimaiVersionId = dbReader.GetInt32(12);
		string tempMaimaiVersionName = dbReader.GetString(13);
		bool tempMaimaiVersionIsDX = dbReader.GetBoolean(14);
		DateTime tempMaimaiVersionLaunchDate = dbReader.GetDateTime(15);
		DateTime? tempMaimaiVersionLaunchDateGlobal = null;
		try
		{
			tempMaimaiVersionLaunchDateGlobal = dbReader.GetDateTime(16);
		}
		catch (InvalidCastException)
		{
			// do nothing if null
		}
		DateTime tempMaimaiVersionCreationDate = dbReader.GetDateTime(17);
		DateTime tempMaimaiVersionLastUpdate = dbReader.GetDateTime(18);
		string tempChartArtist = dbReader.GetString(19);
		string tempChartTitle = dbReader.GetString(20);
		string tempAlbumArtUrl = dbReader.GetString(21);
		int tempChartLevel = dbReader.GetInt32(22);
		bool tempChartIsLevelPlus = dbReader.GetBoolean(23);
		decimal tempChartLevelPrecise = dbReader.GetDecimal(24);
		DateTime tempChartCreationDate = dbReader.GetDateTime(25);
		DateTime tempChartLastUpdate = dbReader.GetDateTime(26);

		return new ChartsTableData()
		{
			ChartID = tempChartId,
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
			Version = new MaimaiVersionsTableData()
			{
				VersionID = tempMaimaiVersionId,
				VersionName = tempMaimaiVersionName,
				IsDeluxe = tempMaimaiVersionIsDX,
				LaunchDate = tempMaimaiVersionLaunchDate,
				LaunchDateGlobal = tempMaimaiVersionLaunchDateGlobal,
				CreationDate = tempMaimaiVersionCreationDate,
				LastUpdate = tempMaimaiVersionLastUpdate
			},
			Artist = tempChartArtist,
			Title = tempChartTitle,
			AlbumArtURL = tempAlbumArtUrl,
			Level = tempChartLevel,
			IsLevelPlus = tempChartIsLevelPlus,
			LevelPrecise = tempChartLevelPrecise,
			CreationDate = tempChartCreationDate,
			LastUpdate = tempChartLastUpdate
		};
	}
}
