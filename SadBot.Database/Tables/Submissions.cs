// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

using System.Data;
using Npgsql;
using SadBot.Utils;

namespace SadBot.Database.Tables;

public struct SubmissionsTableData
{
	public int SubmissionID { get; set; }
	public UsersTableData User { get; set; }
	public ChartsTableData Chart { get; set; }
	public decimal Achievement { get; set; }
	public DateTime CreationDate { get; set; } // set as UTC
	public DateTime LastUpdate { get; set; } // set as UTC
}

public static class Submissions
{
	public static async Task<SubmissionsTableData[]> GetAllSubmissionsAsync(DatabaseTransaction transaction)
	{
		const string query = @"
			SELECT
				submissions.""submission_id"",
				users.""user_id"",
				users.""discord_id"",
				users.""username"",
				users.""discriminator"",
				users.""creation_date"",
				users.""last_update"",
				charts.""chart_id"",
				guilds.""guild_id"",
				guilds.""discord_id"" AS guild_discord_id,
				guilds.""guild_name"",
				guilds.""creation_date"" AS guild_creation_date,
				guilds.""last_update"" AS guild_last_update,
				diffs.""diff_id"",
				diffs.""diff_name"",
				diffs.""diff_short_name"",
				diffs.""color"",
				diffs.""creation_date"" AS diff_creation_date,
				diffs.""last_update"" AS diff_last_update,
				maimai_versions.""version_id"",
				maimai_versions.""version_name"",
				maimai_versions.""is_deluxe"",
				maimai_versions.""launch_date"",
				maimai_versions.""launch_date_global"",
				maimai_versions.""creation_date"" AS version_creation_date,
				maimai_versions.""last_update"" AS version_last_update,
				charts.""artist"",
				charts.""title"",
				charts.""album_art_url"",
				charts.""level"",
				charts.""is_level_plus"",
				charts.""level_precise"",
				charts.""creation_date"" AS chart_creation_date,
				charts.""last_update"" AS chart_last_update,
				submissions.""achievement"",
				submissions.""creation_date"" AS submission_creation_date,
				submissions.""last_update"" AS submission_last_update
			FROM
				submissions
			JOIN
				users ON users.""user_id"" = submissions.""user_id""
			JOIN
				charts ON charts.""chart_id"" = submissions.""chart_id""
			JOIN
				guilds ON guilds.""guild_id"" = charts.""guild_id""
			JOIN
				diffs ON diffs.""diff_id"" = charts.""diff_id""
			JOIN
				maimai_versions ON maimai_versions.""version_id"" = charts.""version_id""
			ORDER BY submissions.""submission_id""
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction);
		await using NpgsqlDataReader dbReader = await dbCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

		if (!dbReader.HasRows)
		{
			Log.WriteVerbose("submissions: Returned 0 rows.");
			return Array.Empty<SubmissionsTableData>();
		}

		List<SubmissionsTableData> ret = new List<SubmissionsTableData>();
		while (await dbReader.ReadAsync())
		{
			ret.Add(GetRowData(dbReader));
		}

		Log.WriteVerbose($"submissions: Returned {ret.Count} row{(ret.Count == 1 ? string.Empty : "s")}.");
		return ret.ToArray();
	}

	public static async Task<SubmissionsTableData[]> GetAllSubmissionsByGuildIDAsync(DatabaseTransaction transaction, int guildId)
	{
		const string query = @"
			SELECT
				submissions.""submission_id"",
				users.""user_id"",
				users.""discord_id"",
				users.""username"",
				users.""discriminator"",
				users.""creation_date"",
				users.""last_update"",
				charts.""chart_id"",
				guilds.""guild_id"",
				guilds.""discord_id"" AS guild_discord_id,
				guilds.""guild_name"",
				guilds.""creation_date"" AS guild_creation_date,
				guilds.""last_update"" AS guild_last_update,
				diffs.""diff_id"",
				diffs.""diff_name"",
				diffs.""diff_short_name"",
				diffs.""color"",
				diffs.""creation_date"" AS diff_creation_date,
				diffs.""last_update"" AS diff_last_update,
				maimai_versions.""version_id"",
				maimai_versions.""version_name"",
				maimai_versions.""is_deluxe"",
				maimai_versions.""launch_date"",
				maimai_versions.""launch_date_global"",
				maimai_versions.""creation_date"" AS version_creation_date,
				maimai_versions.""last_update"" AS version_last_update,
				charts.""artist"",
				charts.""title"",
				charts.""album_art_url"",
				charts.""level"",
				charts.""is_level_plus"",
				charts.""level_precise"",
				charts.""creation_date"" AS chart_creation_date,
				charts.""last_update"" AS chart_last_update,
				submissions.""achievement"",
				submissions.""creation_date"" AS submission_creation_date,
				submissions.""last_update"" AS submission_last_update
			FROM
				submissions
			JOIN
				users ON users.""user_id"" = submissions.""user_id""
			JOIN
				charts ON charts.""chart_id"" = submissions.""chart_id""
			JOIN
				guilds ON guilds.""guild_id"" = charts.""guild_id""
			JOIN
				diffs ON diffs.""diff_id"" = charts.""diff_id""
			JOIN
				maimai_versions ON maimai_versions.""version_id"" = charts.""version_id""
			WHERE guilds.""guild_id"" = ($1)
			ORDER BY submissions.""submission_id""
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
			Log.WriteVerbose("submissions: Returned 0 rows.");
			return Array.Empty<SubmissionsTableData>();
		}

		List<SubmissionsTableData> ret = new List<SubmissionsTableData>();
		while (await dbReader.ReadAsync())
		{
			ret.Add(GetRowData(dbReader));
		}

		Log.WriteVerbose($"submissions: Returned {ret.Count} row{(ret.Count == 1 ? string.Empty : "s")}.");
		return ret.ToArray();
	}

	public static async Task<SubmissionsTableData[]> GetAllSubmissionsByGuildDiscordIDAsync(DatabaseTransaction transaction, string guildDiscordId)
	{
		const string query = @"
			SELECT
				submissions.""submission_id"",
				users.""user_id"",
				users.""discord_id"",
				users.""username"",
				users.""discriminator"",
				users.""creation_date"",
				users.""last_update"",
				charts.""chart_id"",
				guilds.""guild_id"",
				guilds.""discord_id"" AS guild_discord_id,
				guilds.""guild_name"",
				guilds.""creation_date"" AS guild_creation_date,
				guilds.""last_update"" AS guild_last_update,
				diffs.""diff_id"",
				diffs.""diff_name"",
				diffs.""diff_short_name"",
				diffs.""color"",
				diffs.""creation_date"" AS diff_creation_date,
				diffs.""last_update"" AS diff_last_update,
				maimai_versions.""version_id"",
				maimai_versions.""version_name"",
				maimai_versions.""is_deluxe"",
				maimai_versions.""launch_date"",
				maimai_versions.""launch_date_global"",
				maimai_versions.""creation_date"" AS version_creation_date,
				maimai_versions.""last_update"" AS version_last_update,
				charts.""artist"",
				charts.""title"",
				charts.""album_art_url"",
				charts.""level"",
				charts.""is_level_plus"",
				charts.""level_precise"",
				charts.""creation_date"" AS chart_creation_date,
				charts.""last_update"" AS chart_last_update,
				submissions.""achievement"",
				submissions.""creation_date"" AS submission_creation_date,
				submissions.""last_update"" AS submission_last_update
			FROM
				submissions
			JOIN
				users ON users.""user_id"" = submissions.""user_id""
			JOIN
				charts ON charts.""chart_id"" = submissions.""chart_id""
			JOIN
				guilds ON guilds.""guild_id"" = charts.""guild_id""
			JOIN
				diffs ON diffs.""diff_id"" = charts.""diff_id""
			JOIN
				maimai_versions ON maimai_versions.""version_id"" = charts.""version_id""
			WHERE guilds.""discord_id"" = ($1)
			ORDER BY submissions.""submission_id""
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
			Log.WriteVerbose("submissions: Returned 0 rows.");
			return Array.Empty<SubmissionsTableData>();
		}

		List<SubmissionsTableData> ret = new List<SubmissionsTableData>();
		while (await dbReader.ReadAsync())
		{
			ret.Add(GetRowData(dbReader));
		}

		Log.WriteVerbose($"submissions: Returned {ret.Count} row{(ret.Count == 1 ? string.Empty : "s")}.");
		return ret.ToArray();
	}

	public static async Task<SubmissionsTableData?> GetSubmissionBySubmissionIDAsync(DatabaseTransaction transaction, int submissionId)
	{
		const string query = @"
			SELECT
				submissions.""submission_id"",
				users.""user_id"",
				users.""discord_id"",
				users.""username"",
				users.""discriminator"",
				users.""creation_date"",
				users.""last_update"",
				charts.""chart_id"",
				guilds.""guild_id"",
				guilds.""discord_id"" AS guild_discord_id,
				guilds.""guild_name"",
				guilds.""creation_date"" AS guild_creation_date,
				guilds.""last_update"" AS guild_last_update,
				diffs.""diff_id"",
				diffs.""diff_name"",
				diffs.""diff_short_name"",
				diffs.""color"",
				diffs.""creation_date"" AS diff_creation_date,
				diffs.""last_update"" AS diff_last_update,
				maimai_versions.""version_id"",
				maimai_versions.""version_name"",
				maimai_versions.""is_deluxe"",
				maimai_versions.""launch_date"",
				maimai_versions.""launch_date_global"",
				maimai_versions.""creation_date"" AS version_creation_date,
				maimai_versions.""last_update"" AS version_last_update,
				charts.""artist"",
				charts.""title"",
				charts.""album_art_url"",
				charts.""level"",
				charts.""is_level_plus"",
				charts.""level_precise"",
				charts.""creation_date"" AS chart_creation_date,
				charts.""last_update"" AS chart_last_update,
				submissions.""achievement"",
				submissions.""creation_date"" AS submission_creation_date,
				submissions.""last_update"" AS submission_last_update
			FROM
				submissions
			JOIN
				users ON users.""user_id"" = submissions.""user_id""
			JOIN
				charts ON charts.""chart_id"" = submissions.""chart_id""
			JOIN
				guilds ON guilds.""guild_id"" = charts.""guild_id""
			JOIN
				diffs ON diffs.""diff_id"" = charts.""diff_id""
			JOIN
				maimai_versions ON maimai_versions.""version_id"" = charts.""version_id""
			WHERE submissions.""submission_id"" = ($1)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = submissionId }
			}
		};
		await using NpgsqlDataReader dbReader = await dbCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

		if (!dbReader.HasRows)
		{
			Log.WriteWarning($"submissions: Submission with submission_id = {submissionId} not found. Returning as null.");
			return null;
		}

		if (dbReader.Rows > 1)
		{
			throw new DuplicateRecordException("submissions", "submission_id", submissionId.ToString(), dbReader.Rows);
		}

		_ = await dbReader.ReadAsync();

		SubmissionsTableData ret = GetRowData(dbReader);

		Log.WriteVerbose("submissions: Returned 1 row.");
		return ret;
	}

	public static async Task InsertSubmissionAsync(DatabaseTransaction transaction, int userId, int chartId, decimal achievement)
	{
		const string query = @"
			INSERT INTO submissions(user_id, chart_id, achievement)
				VALUES ($1, $2, $3)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = userId },
				new NpgsqlParameter() { Value = chartId },
				new NpgsqlParameter() { Value = achievement }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (user_id = {userId}, chart_id = {chartId}, achievement = {achievement}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("submissions: Inserted 1 row.");
	}

	public static async Task InsertSubmissionAsync(DatabaseTransaction transaction, int userId, int chartId, decimal achievement, int submissionId)
	{
		const string query = @"
			INSERT INTO submissions(submission_id, user_id, chart_id, achievement)
				VALUES ($1, $2, $3, $4)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = submissionId },
				new NpgsqlParameter() { Value = userId },
				new NpgsqlParameter() { Value = chartId },
				new NpgsqlParameter() { Value = achievement }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (submission_id = {submissionId}, user_id = {userId}, chart_id = {chartId}, achievement = {achievement}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("submissions: Inserted 1 row.");
	}

	public static async Task InsertSubmissionAsync(DatabaseTransaction transaction, int userId, int chartId, decimal achievement, DateTime creationDate, DateTime lastUpdate)
	{
		const string query = @"
			INSERT INTO submissions(user_id, chart_id, achievement, creation_date, last_update)
				VALUES ($1, $2, $3, $4, $5)
		";

		DateTime tempUtcCreationDate = creationDate.ToUniversalTime();
		DateTime tempUtcLastUpdate = lastUpdate.ToUniversalTime();

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = userId },
				new NpgsqlParameter() { Value = chartId },
				new NpgsqlParameter() { Value = achievement },
				new NpgsqlParameter() { Value = tempUtcCreationDate },
				new NpgsqlParameter() { Value = tempUtcLastUpdate }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (user_id = {userId}, chart_id = {chartId}, achievement = {achievement}, creation_date = {tempUtcCreationDate}, last_update = {tempUtcLastUpdate}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("submissions: Inserted 1 row.");
	}

	public static async Task InsertSubmissionAsync(DatabaseTransaction transaction, int userId, int chartId, decimal achievement, DateTime creationDate, DateTime lastUpdate, int submissionId)
	{
		const string query = @"
			INSERT INTO submissions(submission_id, user_id, chart_id, achievement, creation_date, last_update)
				VALUES ($1, $2, $3, $4, $5, $6)
		";

		DateTime tempUtcCreationDate = creationDate.ToUniversalTime();
		DateTime tempUtcLastUpdate = lastUpdate.ToUniversalTime();

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = submissionId },
				new NpgsqlParameter() { Value = userId },
				new NpgsqlParameter() { Value = chartId },
				new NpgsqlParameter() { Value = achievement },
				new NpgsqlParameter() { Value = tempUtcCreationDate },
				new NpgsqlParameter() { Value = tempUtcLastUpdate }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (submission_id = {submissionId}, user_id = {userId}, chart_id = {chartId}, achievement = {achievement}, creation_date = {tempUtcCreationDate}, last_update = {tempUtcLastUpdate}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("submissions: Inserted 1 row.");
	}

	public static async Task UpdateSubmissionAsync(DatabaseTransaction transaction, int submissionId, decimal achievement)
	{
		const string query = @"
			UPDATE submissions
			SET achievement = ($1), last_update = (NOW() AT TIME ZONE 'UTC')
			WHERE submission_id = ($2)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = achievement },
				new NpgsqlParameter() { Value = submissionId }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Update query failed (submission_id = {submissionId} -> achievement = {achievement}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Update query failed.");
		}

		Log.WriteVerbose("submissions: Updated 1 row.");
	}

	public static async Task DeleteSubmissionAsync(DatabaseTransaction transaction, int submissionId)
	{
		const string query = @"
			DELETE FROM submissions
			WHERE submission_id = ($1)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = submissionId }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Deletion query failed (submission_id = {submissionId}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Deletion query failed.");
		}

		Log.WriteVerbose("submissions: Deleted 1 row.");
	}

	public static async Task CreateSubmissionsTable(DatabaseTransaction transaction)
	{
		const string query = @"
			CREATE TABLE submissions (
				submission_id SERIAL PRIMARY KEY,
				user_id INTEGER NOT NULL,
				chart_id INTEGER NOT NULL,
				achievement NUMERIC(7, 4) NOT NULL,
				creation_date TIMESTAMP NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
				last_update TIMESTAMP NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
				CONSTRAINT fk_user
					FOREIGN KEY(user_id) REFERENCES users(user_id),
				CONSTRAINT fk_chart
					FOREIGN KEY(chart_id) REFERENCES charts(chart_id)
			)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction);
		_ = await dbCommand.ExecuteNonQueryAsync();
	}

	private static SubmissionsTableData GetRowData(NpgsqlDataReader dbReader)
	{
		int tempSubmissionId = dbReader.GetInt32(0);
		int tempUserId = dbReader.GetInt32(1);
		string tempUserDiscordId = dbReader.GetString(2);
		string tempUsername = dbReader.GetString(3);
		string tempUserDiscriminator = dbReader.GetString(4);
		DateTime tempUserCreationDate = dbReader.GetDateTime(5);
		DateTime tempUserLastUpdate = dbReader.GetDateTime(6);
		int tempChartId = dbReader.GetInt32(7);
		int tempGuildId = dbReader.GetInt32(8);
		string tempGuildDiscordId = dbReader.GetString(9);
		string tempGuildName = dbReader.GetString(10);
		DateTime tempGuildCreationDate = dbReader.GetDateTime(11);
		DateTime tempGuildLastUpdate = dbReader.GetDateTime(12);
		int tempDiffId = dbReader.GetInt32(13);
		string tempDiffName = dbReader.GetString(14);
		string tempDiffShortName = dbReader.GetString(15);
		string tempDiffColor = dbReader.GetString(16);
		DateTime tempDiffCreationDate = dbReader.GetDateTime(17);
		DateTime tempDiffLastUpdate = dbReader.GetDateTime(18);
		int tempMaimaiVersionId = dbReader.GetInt32(19);
		string tempMaimaiVersionName = dbReader.GetString(20);
		bool tempMaimaiVersionIsDX = dbReader.GetBoolean(21);
		DateTime tempMaimaiVersionLaunchDate = dbReader.GetDateTime(22);
		DateTime? tempMaimaiVersionLaunchDateGlobal = null;
		try
		{
			tempMaimaiVersionLaunchDateGlobal = dbReader.GetDateTime(23);
		}
		catch (InvalidCastException)
		{
			// do nothing if null
		}
		DateTime tempMaimaiVersionCreationDate = dbReader.GetDateTime(24);
		DateTime tempMaimaiVersionLastUpdate = dbReader.GetDateTime(25);
		string tempChartArtist = dbReader.GetString(26);
		string tempChartTitle = dbReader.GetString(27);
		string tempAlbumArtUrl = dbReader.GetString(28);
		int tempChartLevel = dbReader.GetInt32(29);
		bool tempChartIsLevelPlus = dbReader.GetBoolean(30);
		decimal tempChartLevelPrecise = dbReader.GetDecimal(31);
		DateTime tempChartCreationDate = dbReader.GetDateTime(32);
		DateTime tempChartLastUpdate = dbReader.GetDateTime(33);
		decimal tempAchievement = dbReader.GetDecimal(34);
		DateTime tempSubmissionCreationDate = dbReader.GetDateTime(35);
		DateTime tempSubmissionLastUpdate = dbReader.GetDateTime(36);

		return new SubmissionsTableData()
		{
			SubmissionID = tempSubmissionId,
			User = new UsersTableData()
			{
				UserID = tempUserId,
				DiscordID = tempUserDiscordId,
				Username = tempUsername,
				Discriminator = tempUserDiscriminator,
				CreationDate = tempUserCreationDate,
				LastUpdate = tempUserLastUpdate
			},
			Chart = new ChartsTableData()
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
			},
			Achievement = tempAchievement,
			CreationDate = tempSubmissionCreationDate,
			LastUpdate = tempSubmissionLastUpdate
		};
	}
}
