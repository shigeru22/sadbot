// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

using System.Data;
using Npgsql;
using SadBot.Utils;

namespace SadBot.Database.Tables;

public struct DifficultiesTableData
{
	public int DiffID { get; set; }
	public string DiffName { get; set; }
	public string DiffShortName { get; set; }
	public string Color { get; set; }
	public DateTime CreationDate { get; set; } // set as UTC
	public DateTime LastUpdate { get; set; } // set as UTC
}

public enum DiffName
{
	EASY = 0,
	BASIC,
	ADVANCED,
	EXPERT,
	MASTER,
	RE_MASTER
}

public static class Difficulties
{
	private static readonly string[] dbDiffNames = { "Easy", "Basic", "Advanced", "Expert", "Master", "Re:Master" };
	private static readonly string[] dbDiffShortNames = { "EAS", "BAS", "ADV", "EXP", "MAS", "RMS" };

	public static async Task<DifficultiesTableData[]> GetAllDifficultiesAsync(DatabaseTransaction transaction)
	{
		const string query = @"
			SELECT
				diffs.""diff_id"",
				diffs.""diff_name"",
				diffs.""diff_short_name"",
				diffs.""color"",
				diffs.""creation_date"",
				diffs.""last_update""
			FROM
				diffs
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction);
		await using NpgsqlDataReader dbReader = await dbCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

		if (!dbReader.HasRows)
		{
			Log.WriteVerbose("diffs: Returned 0 rows.");
			return Array.Empty<DifficultiesTableData>();
		}

		List<DifficultiesTableData> ret = new List<DifficultiesTableData>();
		while (await dbReader.ReadAsync())
		{
			ret.Add(new DifficultiesTableData()
			{
				DiffID = dbReader.GetInt32(0),
				DiffName = dbReader.GetString(1),
				DiffShortName = dbReader.GetString(2),
				Color = $"#{dbReader.GetString(3)}", // directly prepend '#' here
				CreationDate = dbReader.GetDateTime(4),
				LastUpdate = dbReader.GetDateTime(5)
			});
		}

		Log.WriteVerbose($"guilds: Returned {ret.Count} row{(ret.Count == 1 ? string.Empty : "s")}.");
		return ret.ToArray();
	}

	public static async Task<DifficultiesTableData?> GetDifficultyByDiffIDAsync(DatabaseTransaction transaction, int dbDiffId)
	{
		const string query = @"
			SELECT
				diffs.""diff_id"",
				diffs.""diff_name"",
				diffs.""diff_short_name"",
				diffs.""color"",
				diffs.""creation_date"",
				diffs.""last_update""
			FROM
				diffs
			WHERE
				diffs.""diff_id"" = ($1)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = dbDiffId }
			}
		};
		await using NpgsqlDataReader dbReader = await dbCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

		if (!dbReader.HasRows)
		{
			Log.WriteWarning($"diffs: Difficulty with diff_id = {dbDiffId} not found. Returning as null.");
			return null;
		}

		if (dbReader.Rows > 1)
		{
			throw new DuplicateRecordException("diffs", "diff_id", dbDiffId.ToString(), dbReader.Rows);
		}

		_ = await dbReader.ReadAsync();

		DifficultiesTableData ret = new DifficultiesTableData()
		{
			DiffID = dbReader.GetInt32(0),
			DiffName = dbReader.GetString(1),
			DiffShortName = dbReader.GetString(2),
			Color = $"#{dbReader.GetString(3)}", // directly prepend '#' here
			CreationDate = dbReader.GetDateTime(4),
			LastUpdate = dbReader.GetDateTime(5)
		};

		Log.WriteVerbose("diffs: Returned 1 row.");
		return ret;
	}

	public static async Task<DifficultiesTableData?> GetDifficultyByDiffNameAsync(DatabaseTransaction transaction, DiffName diffName)
	{
		const string query = @"
			SELECT
				diffs.""diff_id"",
				diffs.""diff_name"",
				diffs.""diff_short_name"",
				diffs.""color"",
				diffs.""creation_date"",
				diffs.""last_update""
			FROM
				diffs
			WHERE
				diffs.""diff_name"" = ($1)
		";

		string tempDiffName = dbDiffNames[(int)diffName];

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = tempDiffName }
			}
		};
		await using NpgsqlDataReader dbReader = await dbCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

		if (!dbReader.HasRows)
		{
			Log.WriteWarning($"diffs: Difficulty with diff_name = {tempDiffName} not found. Returning as null.");
			return null;
		}

		if (dbReader.Rows > 1)
		{
			throw new DuplicateRecordException("diffs", "diff_name", tempDiffName, dbReader.Rows);
		}

		_ = await dbReader.ReadAsync();

		DifficultiesTableData ret = new DifficultiesTableData()
		{
			DiffID = dbReader.GetInt32(0),
			DiffName = dbReader.GetString(1),
			DiffShortName = dbReader.GetString(2),
			Color = $"#{dbReader.GetString(3)}", // directly prepend '#' here
			CreationDate = dbReader.GetDateTime(4),
			LastUpdate = dbReader.GetDateTime(5)
		};

		Log.WriteVerbose("diffs: Returned 1 row.");
		return ret;
	}

	public static async Task InsertDifficultyAsync(DatabaseTransaction transaction, DiffName diffName, string diffColor)
	{
		const string query = @"
			INSERT INTO diffs (diff_name, diff_short_name, color)
				VALUES ($1, $2, $3)
		";

		string tempDiffName = dbDiffNames[(int)diffName];
		string tempDiffShortName = dbDiffShortNames[(int)diffName];
		string tempColor = diffColor.Replace("#", string.Empty);

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = tempDiffName },
				new NpgsqlParameter() { Value = tempDiffShortName },
				new NpgsqlParameter() { Value = tempColor }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (diff_name = {tempDiffName}, diff_short_name = {tempDiffShortName}, color = {tempColor}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("diffs: Inserted 1 row.");
	}

	public static async Task InsertDifficultyAsync(DatabaseTransaction transaction, DiffName diffName, string diffColor, int diffId)
	{
		const string query = @"
			INSERT INTO diffs (diff_id, diff_name, diff_short_name, color)
				VALUES ($1, $2, $3, $4)
		";

		string tempDiffName = dbDiffNames[(int)diffName];
		string tempDiffShortName = dbDiffShortNames[(int)diffName];
		string tempColor = diffColor.Replace("#", string.Empty);

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = diffId },
				new NpgsqlParameter() { Value = tempDiffName },
				new NpgsqlParameter() { Value = tempDiffShortName },
				new NpgsqlParameter() { Value = tempColor }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (diff_name = {tempDiffName}, diff_short_name = {tempDiffShortName}, color = {tempColor}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("diffs: Inserted 1 row.");
	}

	public static async Task InsertDifficultyAsync(DatabaseTransaction transaction, DiffName diffName, string diffColor, DateTime creationDate, DateTime lastUpdate)
	{
		const string query = @"
			INSERT INTO diffs (diff_name, diff_short_name, color, creation_date, last_update)
				VALUES ($1, $2, $3, $4, $5)
		";

		string tempDiffName = dbDiffNames[(int)diffName];
		string tempDiffShortName = dbDiffShortNames[(int)diffName];
		string tempColor = diffColor.Replace("#", string.Empty);
		DateTime tempUtcCreationDate = creationDate.ToUniversalTime();
		DateTime tempUtcLastUpdate = lastUpdate.ToUniversalTime();

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = tempDiffName },
				new NpgsqlParameter() { Value = tempDiffShortName },
				new NpgsqlParameter() { Value = tempColor },
				new NpgsqlParameter() { Value = tempUtcCreationDate },
				new NpgsqlParameter() { Value = tempUtcLastUpdate }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (diff_name = {tempDiffName}, diff_short_name = {tempDiffShortName}, color = {tempColor}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("diffs: Inserted 1 row.");
	}

	public static async Task InsertDifficultyAsync(DatabaseTransaction transaction, DiffName diffName, string diffColor, DateTime creationDate, DateTime lastUpdate, int diffId)
	{
		const string query = @"
			INSERT INTO diffs (diff_id, diff_name, diff_short_name, color, creation_date, last_update)
				VALUES ($1, $2, $3, $4, $5, $6)
		";

		string tempDiffName = dbDiffNames[(int)diffName];
		string tempDiffShortName = dbDiffShortNames[(int)diffName];
		string tempColor = diffColor.Replace("#", string.Empty);
		DateTime tempUtcCreationDate = creationDate.ToUniversalTime();
		DateTime tempUtcLastUpdate = lastUpdate.ToUniversalTime();

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = diffId },
				new NpgsqlParameter() { Value = tempDiffName },
				new NpgsqlParameter() { Value = tempDiffShortName },
				new NpgsqlParameter() { Value = tempColor },
				new NpgsqlParameter() { Value = tempUtcCreationDate },
				new NpgsqlParameter() { Value = tempUtcLastUpdate }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (diff_name = {tempDiffName}, diff_short_name = {tempDiffShortName}, color = {tempColor}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("diffs: Inserted 1 row.");
	}

	public static async Task UpdateDifficultyAsync(DatabaseTransaction transaction, int diffId, string diffColor)
	{
		const string query = @"
			UPDATE diffs
			SET color = ($1), last_update = (NOW() AT TIME ZONE 'UTC')
			WHERE diff_id = ($2)
		";

		string tempDiffColor = diffColor.Replace("#", string.Empty);

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = tempDiffColor },
				new NpgsqlParameter() { Value = diffId }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Update query failed (color = {tempDiffColor}, diff_id = {diffId}.");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Update query failed.");
		}

		Log.WriteVerbose("diffs: Updated 1 row.");
	}

	public static async Task UpdateDifficultyAsync(DatabaseTransaction transaction, DiffName diffName, string diffColor)
	{
		const string query = @"
			UPDATE diffs
			SET color = ($1), last_update = (NOW() AT TIME ZONE 'UTC')
			WHERE diff_name = ($2)
		";

		string tempDiffName = dbDiffNames[(int)diffName];
		string tempDiffColor = diffColor.Replace("#", string.Empty);

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = tempDiffColor },
				new NpgsqlParameter() { Value = tempDiffName }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Update query failed (color = {tempDiffColor}, diff_name = {tempDiffName}.");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Update query failed.");
		}

		Log.WriteVerbose("diffs: Updated 1 row.");
	}

	public static async Task DeleteDifficultyAsync(DatabaseTransaction transaction, int diffId)
	{
		const string query = @"
			DELETE FROM diffs
			WHERE diff_id = ($1)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = diffId }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Deletion query failed (diff_id = {diffId})");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Deletion query failed.");
		}

		Log.WriteVerbose("diffs: Deleted 1 row.");
	}

	public static async Task DeleteDifficultyAsync(DatabaseTransaction transaction, DiffName diffName)
	{
		const string query = @"
			DELETE FROM diffs
			WHERE diff_name = ($1)
		";

		string tempDiffName = dbDiffNames[(int)diffName];

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = tempDiffName }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Deletion query failed (diff_name = {tempDiffName})");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Deletion query failed.");
		}

		Log.WriteVerbose("diffs: Deleted 1 row.");
	}

	public static async Task CreateGuildsTable(DatabaseTransaction transaction)
	{
		const string query = @"
			CREATE TABLE diffs (
				diff_id SERIAL PRIMARY KEY,
				diff_name VARCHAR(255),
				diff_short_name VARCHAR(3),
				color VARCHAR(6),
				creation_date TIMESTAMP NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
				last_update TIMESTAMP NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC')
			)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction);
		_ = await dbCommand.ExecuteNonQueryAsync();
	}
}
