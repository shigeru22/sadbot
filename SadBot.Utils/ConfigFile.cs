// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace SadBot.Utils;

internal readonly struct ConfigFileContents
{
	[JsonPropertyName("botToken")] public string BotToken { get; init; }
	[JsonPropertyName("useReply")] public bool UseReply { get; init; }
	[JsonPropertyName("logging")] public ConfigFileLoggingContents Logging { get; init; }
	[JsonPropertyName("database")] public ConfigFileDatabaseContents Database { get; init; }
}

internal readonly struct ConfigFileLoggingContents
{
	[JsonPropertyName("useUtc")] public bool UseUTC { get; init; }
	[JsonPropertyName("logSeverity")] public int LogSeverity { get; init; }
}

internal readonly struct ConfigFileDatabaseContents
{
	[JsonPropertyName("hostname")] public string Hostname { get; init; }
	[JsonPropertyName("port")] public int Port { get; init; }
	[JsonPropertyName("username")] public string Username { get; init; }
	[JsonPropertyName("password")] public string Password { get; init; }
	[JsonPropertyName("name")] public string Name { get; init; }
	[JsonPropertyName("certPath")] public string? CertificatePath { get; init; }
	[JsonPropertyName("sslMode")] public string? UseSSL { get; init; }
}

internal class ConfigFile
{
	private static readonly ConfigFile instance = new ConfigFile();

	public static ConfigFile Instance => instance;

	private const string CONFIG_FILE_PATH = "appsettings.json";

	public string? BotToken { get; private set; }
	public bool? UseReply { get; private set; }
	public bool? UseUTC { get; private set; }
	public int? LogSeverity { get; private set; }
	public string? DatabaseHost { get; private set; }
	public int? DatabasePort { get; private set; }
	public string? DatabaseUsername { get; private set; }
	public string? DatabasePassword { get; private set; }
	public string? DatabaseName { get; private set; }
	public string? DatabaseCertificatePath { get; private set; }
	public string? DatabaseUseSSL { get; private set; }

	private ConfigFile()
	{
		Log.WriteVerbose("Config file wrapper instance created.");
		PopulateConfigFile();
	}

	private void PopulateConfigFile()
	{
		Log.WriteVerbose("Populating config file to current config file instance.");

		if (!File.Exists(CONFIG_FILE_PATH))
		{
			Log.WriteVerbose("Config file not found. Using null for all configurations.");
			return;
		}

		ConfigFileContents tempConfig = JsonSerializer.Deserialize<ConfigFileContents>(File.ReadAllText(CONFIG_FILE_PATH));

		Log.WriteDebug("Setting instance configurations with config file data.");

		BotToken = tempConfig.BotToken;
		UseReply = tempConfig.UseReply;
		UseUTC = tempConfig.Logging.UseUTC;
		LogSeverity = tempConfig.Logging.LogSeverity;
		DatabaseHost = tempConfig.Database.Hostname;
		DatabasePort = tempConfig.Database.Port;
		DatabaseUsername = tempConfig.Database.Username;
		DatabasePassword = tempConfig.Database.Password;
		DatabaseName = tempConfig.Database.Name;
		DatabaseCertificatePath = tempConfig.Database.CertificatePath;
		DatabaseUseSSL = tempConfig.Database.UseSSL;
	}
}
