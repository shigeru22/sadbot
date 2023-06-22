// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

using System.Text;
using SadBot.Utils;

namespace SadBot.Database;

public class DatabaseConfig
{
	public string? Hostname { get; set; }
	public int? Port { get; set; }
	public string? Username { get; set; }
	public string? Password { get; set; }
	public string? DatabaseName { get; set; }
	public string? CertificatePath { get; set; }
	public bool UseSSL { get; set; }

	public string ToConnectionString()
	{
		bool isError = false;

		if (string.IsNullOrWhiteSpace(Hostname))
		{
			Log.WriteError("Hostname must not be null nor empty nor spaces.");
			isError = true;
		}

		if (Port is < 1 or > 65535)
		{
			Log.WriteError("Port number must be between 1 and 65535.");
			isError = true;
		}

		if (string.IsNullOrWhiteSpace(Username))
		{
			Log.WriteError("Username must not be null nor empty nor spaces.");
			isError = true;
		}

		if (string.IsNullOrWhiteSpace(Password))
		{
			Log.WriteError("Password must not be null nor empty nor spaces.");
			isError = true;
		}

		if (string.IsNullOrWhiteSpace(DatabaseName))
		{
			Log.WriteError("Database name must not be null nor empty nor spaces.");
			isError = true;
		}

		if (isError)
		{
			throw new DatabaseConfigException();
		}

		StringBuilder sbRet = new StringBuilder($"Host={Hostname};Port={Port};Database={DatabaseName};Username={Username};Password={Password}");

		if (string.IsNullOrWhiteSpace(CertificatePath) && !UseSSL)
		{
			Log.WriteWarning("Not using SSL for database connection. Caute procedere.");
		}

		if (!string.IsNullOrWhiteSpace(CertificatePath))
		{
			_ = sbRet.Append($";SSL Certificate={CertificatePath}");
		}
		else if (!UseSSL)
		{
			_ = sbRet.Append(";SSL Mode=Disable");
		}

		return sbRet.ToString();
	}
}
