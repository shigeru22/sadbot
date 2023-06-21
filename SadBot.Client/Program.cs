// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

using SadBot.Utils;

namespace SadBot.Client;

public static class Program
{
	public static void Main(string[] args)
	{
		Log.WriteCritical("This is critical message.");
		Log.WriteError("This is error message.");
		Log.WriteWarning("This is warning message.");
		Log.WriteInfo("This is info message.");
		Log.WriteVerbose("This is verbose message.");
		Log.WriteDebug("This is debug message.");
	}
}
