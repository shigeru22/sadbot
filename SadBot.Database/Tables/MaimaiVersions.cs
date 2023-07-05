// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

using System.Data;
using Npgsql;
using SadBot.Utils;

namespace SadBot.Database.Tables;

public struct MaimaiVersionsTableData
{
	public int VersionID { get; set; }
	public string VersionName { get; set; }
	public bool IsDeluxe { get; set; }
	public DateTime LaunchDate { get; set; } // set as UTC, only date is used
	public DateTime? LaunchDateGlobal { get; set; } // set as UTC, only date is used
	public DateTime CreationDate { get; set; } // set as UTC
	public DateTime LastUpdate { get; set; } // set as UTC
}

public static class MaimaiVersions
{
	public static async Task<MaimaiVersionsTableData[]> GetAllVersionsAsync(DatabaseTransaction transaction)
	{
		const string query = @"
			SELECT
				maimai_versions.""version_id"",
				maimai_versions.""version_name"",
				maimai_versions.""is_deluxe"",
				maimai_versions.""launch_date"",
				maimai_versions.""launch_date_global"",
				maimai_versions.""creation_date"",
				maimai_versions.""last_update""
			FROM
				maimai_versions
			ORDER BY
				maimai_versions.""launch_date""
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction);
		await using NpgsqlDataReader dbReader = await dbCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

		if (!dbReader.HasRows)
		{
			Log.WriteVerbose("maimai_versions: Returned 0 rows.");
			return Array.Empty<MaimaiVersionsTableData>();
		}

		List<MaimaiVersionsTableData> ret = new List<MaimaiVersionsTableData>();
		while (await dbReader.ReadAsync())
		{
			ret.Add(GetRowData(dbReader));
		}

		Log.WriteVerbose($"maimai_versions: Returned {ret.Count} row{(ret.Count == 1 ? string.Empty : "s")}.");
		return ret.ToArray();
	}

	public static async Task<MaimaiVersionsTableData?> GetVersionByVersionIDAsync(DatabaseTransaction transaction, int dbVersionId)
	{
		const string query = @"
			SELECT
				maimai_versions.""version_id"",
				maimai_versions.""version_name"",
				maimai_versions.""is_deluxe"",
				maimai_versions.""launch_date"",
				maimai_versions.""launch_date_global"",
				maimai_versions.""creation_date"",
				maimai_versions.""last_update""
			FROM
				maimai_versions
			WHERE
				maimai_versions.""version_id"" = ($1)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = dbVersionId }
			}
		};
		await using NpgsqlDataReader dbReader = await dbCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

		if (!dbReader.HasRows)
		{
			Log.WriteWarning($"maimai_versions: Version with version_id = {dbVersionId} not found. Returning as null.");
			return null;
		}

		if (dbReader.Rows > 1)
		{
			throw new DuplicateRecordException("maimai_versions", "version_id", dbVersionId.ToString(), dbReader.Rows);
		}

		_ = await dbReader.ReadAsync();

		MaimaiVersionsTableData ret = GetRowData(dbReader);

		Log.WriteVerbose("maimai_versions: Returned 1 row.");
		return ret;
	}

	public static async Task InsertVersionAsync(DatabaseTransaction transaction, string versionName, bool isDeluxe, DateTime launchDate, DateTime? launchDateGlobal)
	{
		const string query = @"
			INSERT INTO maimai_versions (version_name, is_deluxe, launch_date, launch_date_global)
				VALUES ($1, $2, $3, $4)
		";

		DateTime tempUtcLaunchDate = launchDate.ToUniversalTime();
		object tempUtcLaunchDateGlobal = launchDateGlobal.HasValue ? launchDateGlobal.Value.ToUniversalTime() : DBNull.Value;

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = versionName },
				new NpgsqlParameter() { Value = isDeluxe },
				new NpgsqlParameter() { Value = tempUtcLaunchDate },
				new NpgsqlParameter() { Value = tempUtcLaunchDateGlobal }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (version_name = {versionName}, is_deluxe = {isDeluxe}, launch_date = {tempUtcLaunchDate}, launch_date_global = {tempUtcLaunchDateGlobal}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("maimai_versions: Inserted 1 row.");
	}

	public static async Task InsertVersionAsync(DatabaseTransaction transaction, string versionName, bool isDeluxe, DateTime launchDate, DateTime? launchDateGlobal, int versionId)
	{
		const string query = @"
			INSERT INTO maimai_versions (version_id, version_name, is_deluxe, launch_date, launch_date_global)
				VALUES ($1, $2, $3, $4, $5)
		";

		DateTime tempUtcLaunchDate = launchDate.ToUniversalTime();
		object tempUtcLaunchDateGlobal = launchDateGlobal.HasValue ? launchDateGlobal.Value.ToUniversalTime() : DBNull.Value;

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = versionId },
				new NpgsqlParameter() { Value = versionName },
				new NpgsqlParameter() { Value = isDeluxe },
				new NpgsqlParameter() { Value = tempUtcLaunchDate },
				new NpgsqlParameter() { Value = tempUtcLaunchDateGlobal }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (version_id = {versionId}, version_name = {versionName}, is_deluxe = {isDeluxe}, launch_date = {tempUtcLaunchDate}, launch_date_global = {tempUtcLaunchDateGlobal}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("maimai_versions: Inserted 1 row.");
	}

	public static async Task InsertVersionAsync(DatabaseTransaction transaction, string versionName, bool isDeluxe, DateTime launchDate, DateTime? launchDateGlobal, DateTime creationDate, DateTime lastUpdate)
	{
		const string query = @"
			INSERT INTO maimai_versions (version_name, is_deluxe, launch_date, launch_date_global, creation_date, last_update)
				VALUES ($1, $2, $3, $4, $5, $6)
		";

		DateTime tempUtcLaunchDate = launchDate.ToUniversalTime();
		object tempUtcLaunchDateGlobal = launchDateGlobal.HasValue ? launchDateGlobal.Value.ToUniversalTime() : DBNull.Value;
		DateTime tempUtcCreationDate = creationDate.ToUniversalTime();
		DateTime tempUtcLastUpdate = lastUpdate.ToUniversalTime();

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = versionName },
				new NpgsqlParameter() { Value = isDeluxe },
				new NpgsqlParameter() { Value = tempUtcLaunchDate },
				new NpgsqlParameter() { Value = tempUtcLaunchDateGlobal },
				new NpgsqlParameter() { Value = tempUtcCreationDate },
				new NpgsqlParameter() { Value = tempUtcLastUpdate }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (version_name = {versionName}, is_deluxe = {isDeluxe}, launch_date = {tempUtcLaunchDate}, creation_date = {tempUtcCreationDate}, last_update = {tempUtcLastUpdate}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("maimai_versions: Inserted 1 row.");
	}

	public static async Task InsertVersionAsync(DatabaseTransaction transaction, string versionName, bool isDeluxe, DateTime launchDate, DateTime? launchDateGlobal, DateTime creationDate, DateTime lastUpdate, int versionId)
	{
		const string query = @"
			INSERT INTO maimai_versions (version_id, version_name, is_deluxe, launch_date, launch_date_global, creation_date, last_update)
				VALUES ($1, $2, $3, $4, $5, $6, $7)
		";

		DateTime tempUtcLaunchDate = launchDate.ToUniversalTime();
		object tempUtcLaunchDateGlobal = launchDateGlobal.HasValue ? launchDateGlobal.Value.ToUniversalTime() : DBNull.Value;
		DateTime tempUtcCreationDate = creationDate.ToUniversalTime();
		DateTime tempUtcLastUpdate = lastUpdate.ToUniversalTime();

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = versionId },
				new NpgsqlParameter() { Value = versionName },
				new NpgsqlParameter() { Value = isDeluxe },
				new NpgsqlParameter() { Value = tempUtcLaunchDate },
				new NpgsqlParameter() { Value = tempUtcLaunchDateGlobal },
				new NpgsqlParameter() { Value = tempUtcCreationDate },
				new NpgsqlParameter() { Value = tempUtcLastUpdate }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Insertion query failed (version_id = {versionId}, version_name = {versionName}, is_deluxe = {isDeluxe}, launch_date = {tempUtcLaunchDate}, creation_date = {tempUtcCreationDate}, last_update = {tempUtcLastUpdate}).");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Insertion query failed.");
		}

		Log.WriteVerbose("maimai_versions: Inserted 1 row.");
	}

	public static async Task UpdateVersionAsync(DatabaseTransaction transaction, int dbVersionId, string versionName, bool isDeluxe, DateTime launchDate, DateTime? launchDateGlobal)
	{
		const string query = @"
			UPDATE maimai_versions
			SET version_name = ($1), is_deluxe = ($2), launch_date = ($3), launch_date_global = ($4), last_update = (NOW() AT TIME ZONE 'UTC')
			WHERE version_id = ($5)
		";

		DateTime tempUtcLaunchDate = launchDate.ToUniversalTime();
		object tempUtcLaunchDateGlobal = launchDateGlobal.HasValue ? launchDateGlobal.Value.ToUniversalTime() : DBNull.Value;

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = versionName },
				new NpgsqlParameter() { Value = isDeluxe },
				new NpgsqlParameter() { Value = tempUtcLaunchDate },
				new NpgsqlParameter() { Value = tempUtcLaunchDateGlobal },
				new NpgsqlParameter() { Value = dbVersionId }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Update query failed (version_name = {versionName}, is_deluxe = {isDeluxe}, launch_date = {tempUtcLaunchDate}, version_id = {dbVersionId}.");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Update query failed.");
		}

		Log.WriteVerbose("maimai_versions: Updated 1 row.");
	}

	public static async Task DeleteVersionAsync(DatabaseTransaction transaction, int dbVersionId)
	{
		const string query = @"
			DELETE FROM maimai_versions
			WHERE version_id = ($1)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction)
		{
			Parameters =
			{
				new NpgsqlParameter() { Value = dbVersionId }
			}
		};

		int affectedRows = await dbCommand.ExecuteNonQueryAsync();

		if (affectedRows != 1)
		{
			Log.WriteError($"Deletion query failed (version_id = {dbVersionId})");
			throw new DatabaseInstanceException(dbCommand.CommandText, "Deletion query failed.");
		}

		Log.WriteVerbose("maimai_versions: Deleted 1 row.");
	}

	public static async Task CreateVersionsTable(DatabaseTransaction transaction)
	{
		const string query = @"
			CREATE TABLE maimai_versions (
				version_id SERIAL PRIMARY KEY,
				version_name VARCHAR(255),
				is_deluxe BOOLEAN,
				launch_date TIMESTAMP,
				launch_date_global TIMESTAMP,
				creation_date TIMESTAMP NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
				last_update TIMESTAMP NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC')
			)
		";

		await using NpgsqlCommand dbCommand = new NpgsqlCommand(query, transaction.Connection, transaction.Transaction);
		_ = await dbCommand.ExecuteNonQueryAsync();
	}

	private static MaimaiVersionsTableData GetRowData(NpgsqlDataReader dbReader)
	{
		int tempVersionId = dbReader.GetInt32(0);
		string tempVersionName = dbReader.GetString(1);
		bool tempIsDeluxe = dbReader.GetBoolean(2);
		DateTime tempLaunchDate = dbReader.GetDateTime(3);
		DateTime? tempLaunchDateGlobal = null;
		try
		{
			tempLaunchDateGlobal = dbReader.GetDateTime(4);
		}
		catch (InvalidCastException)
		{
			// do nothing if null
		}
		DateTime tempCreationDate = dbReader.GetDateTime(5);
		DateTime tempLastUpdate = dbReader.GetDateTime(6);

		return new MaimaiVersionsTableData()
		{
			VersionID = tempVersionId,
			VersionName = tempVersionName,
			IsDeluxe = tempIsDeluxe,
			LaunchDate = tempLaunchDate,
			LaunchDateGlobal = tempLaunchDateGlobal,
			CreationDate = tempCreationDate,
			LastUpdate = tempLastUpdate
		};
	}
}
