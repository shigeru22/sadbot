// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

namespace SadBot.Utils;

public readonly struct LoggingSettings
{
	public bool UseUTC { get; init; }
	public LogSeverity Severity { get; init; }
}

public readonly struct DatabaseSettings
{
	public string Hostname { get; init; }
	public int Port { get; init; }
	public string Username { get; init; }
	public string Password { get; init; }
	public string Name { get; init; }
	public string? CertificatePath { get; init; }
	public string? SSLMode { get; init; }
}

public class Settings
{
	private static readonly Settings instance = new Settings();

	public static Settings Instance => instance;

	public string BotToken { get; init; }
	public bool UseReply { get; init; }
	public LoggingSettings Logging { get; init; }
	public DatabaseSettings Database { get; init; }

	private Settings()
	{
		Log.WriteVerbose("Settings instantiated. Reading all configurations.");

		// order: env -> config file -> arguments

		EnvironmentWrapper envConfInstance = EnvironmentWrapper.Instance;
		ConfigFile fileConfInstance = ConfigFile.Instance;
		ArgumentsWrapper argConfInstance = ArgumentsWrapper.Instance;

		// combine everything

		string? tempBotToken = argConfInstance.BotToken ?? fileConfInstance.BotToken ?? envConfInstance.BotToken;
		bool tempUseReply = argConfInstance.UseReply ?? fileConfInstance.UseReply ?? envConfInstance.UseReply ?? false;
		bool tempUseUtc = argConfInstance.UseUTC ?? fileConfInstance.UseUTC ?? envConfInstance.UseUTC ?? false;
		int tempLogSeverity = argConfInstance.LogSeverity ?? fileConfInstance.LogSeverity ?? envConfInstance.LogSeverity ?? 3;
		string tempDatabaseHostname = argConfInstance.DatabaseHost ?? fileConfInstance.DatabaseHost ?? envConfInstance.DatabaseHost ?? "localhost";
		int tempDatabasePort = argConfInstance.DatabasePort ?? fileConfInstance.DatabasePort ?? envConfInstance.DatabasePort ?? 5432;
		string tempDatabaseUsername = argConfInstance.DatabaseUsername ?? fileConfInstance.DatabaseUsername ?? envConfInstance.DatabaseUsername ?? "postgres";
		string tempDatabasePassword = argConfInstance.DatabasePassword ?? fileConfInstance.DatabasePassword ?? envConfInstance.DatabasePassword ?? string.Empty;
		string tempDatabaseName = argConfInstance.DatabaseName ?? fileConfInstance.DatabaseName ?? envConfInstance.DatabaseName ?? "postgres";
		string? tempDatabaseCertificatePath = argConfInstance.DatabaseCertificatePath ?? fileConfInstance.DatabaseCertificatePath ?? envConfInstance.DatabaseCertificatePath;
		string? tempDatabaseSslMode = argConfInstance.DatabaseUseSSL ?? fileConfInstance.DatabaseUseSSL ?? envConfInstance.DatabaseUseSSL;

		// check for invalid required settings

		if (string.IsNullOrWhiteSpace(tempBotToken))
		{
			throw new InvalidSettingException("BotToken", "Bot token must be specified.");
		}

		LogSeverity maxSeverityValue = Enum.GetValues(typeof(LogSeverity)).Cast<LogSeverity>().Max();
		if (tempLogSeverity < 0 || tempLogSeverity > (int)maxSeverityValue) // follow LogSeverity
		{
			throw new InvalidSettingException("Logging.Severity", $"Log severity must be between 1 and {(int)maxSeverityValue}.");
		}

		if (tempDatabasePort is < 1 or > ushort.MaxValue)
		{
			throw new InvalidSettingException("Database.Port", $"Database port must be between 1 and {ushort.MaxValue}");
		}

		// all good -- insert to instance properties

		BotToken = tempBotToken;
		UseReply = tempUseReply;
		Logging = new LoggingSettings()
		{
			UseUTC = tempUseUtc,
			Severity = (LogSeverity)tempLogSeverity
		};
		Database = new DatabaseSettings()
		{
			Hostname = tempDatabaseHostname,
			Port = tempDatabasePort,
			Username = tempDatabaseUsername,
			Password = tempDatabasePassword,
			Name = tempDatabaseName,
			CertificatePath = tempDatabaseCertificatePath,
			SSLMode = tempDatabaseSslMode
		};
	}
}
