// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

using Discord;
using Discord.WebSocket;
using SadBot.Database;
using SadBot.Utils;

namespace SadBot.Client;

public class BotClient
{
	private readonly DiscordSocketClient client;
	private readonly string botToken;

	private readonly CancellationTokenSource delayToken = new CancellationTokenSource();
	private readonly object exitMutex = new object();

	public BotClient(string botToken)
	{
		Log.WriteVerbose("BotClient instance created.");

		client = new DiscordSocketClient(new DiscordSocketConfig()
		{
			UseInteractionSnowflakeDate = false,
			GatewayIntents = GatewayIntents.None
		});
		this.botToken = botToken;

		Log.WriteVerbose("Registering process events.");

		client.Log += WriteLogAsync;

		Console.CancelKeyPress += OnProcessExit;
		AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
	}

	public async Task RunAsync()
	{
		Log.WriteInfo("Starting bot client using specified token.");

		await client.LoginAsync(TokenType.Bot, botToken);
		await client.StartAsync();

		Log.WriteVerbose("Client started. Awaiting process indefinitely.");

		await Task.Delay(-1, delayToken.Token);
	}

	private void OnProcessExit()
	{
		lock (exitMutex)
		{
			if (client.ConnectionState == ConnectionState.Connected)
			{
				Log.WriteVerbose("Method called. Logging out client.");

				Task.Run(async () =>
				{
					await client.StopAsync();
					await client.LogoutAsync();
				}).Wait();

				Log.WriteVerbose("Client logged out.");

				delayToken.Cancel();
			}
		}
	}

	private void OnProcessExit(object? o, EventArgs e) => OnProcessExit();

	public static Task WriteLogAsync(LogMessage logMessage)
	{
		Log.Write(
			(Utils.LogSeverity)(int)logMessage.Severity,
			logMessage.Message,
			logMessage.Source
		);
		return Task.CompletedTask;
	}
}
