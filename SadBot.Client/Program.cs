// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

using SadBot.Database;
using SadBot.Utils;

namespace SadBot.Client;

public static class Program
{
	public static async Task Main(string[] args)
	{
		Log.WriteInfo("Program started.");
		Log.WriteVerbose("Creating bot client instance.");
		BotClient client = new BotClient(Settings.Instance.BotToken);
		await client.RunAsync();
	}
}
