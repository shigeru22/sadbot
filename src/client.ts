// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

import { Client, GatewayIntentBits } from "discord.js";
import { Log } from "./utils/log.js";

export class BotClient {
  private client: Client;

  private botToken: string;

  constructor(botToken: string) {
    Log.writeVerbose("BotClient", "Client instantiated. Setting up instance.");

    this.botToken = botToken;
    this.client = new Client({
      intents: GatewayIntentBits.Guilds | GatewayIntentBits.GuildMessages | GatewayIntentBits.MessageContent
    });

    Log.writeVerbose("BotClient", "Registering process and client events.");

    process.on("SIGINT", () => this.onExit());
    process.on("SIGTERM", () => this.onExit());

    this.client.on("ready", () => this.onReady());

    Log.writeVerbose("BotClient", "Client initialized.");
  }

  public async run() {
    Log.writeVerbose("run", "Starting client using specified bot token.");
    await this.client.login(this.botToken);
  }

  private onReady() {
    Log.writeInfo("onReady", "Bot client started.");
  }

  private onExit() {
    Log.writeInfo("onExit", "Exit signal received. Cleaning up process...");
    this.client.destroy();
    Log.writeInfo("onExit", "Cleanup success. Exiting...");

    process.exit(0);
  }
}
