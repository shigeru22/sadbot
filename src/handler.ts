// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

import { Guild, Message, Interaction, CacheType, ChatInputCommandInteraction } from "discord.js";
import { Log } from "./utils/log.js";

export class Handler {
  public static onJoinGuild(guild: Guild) {
    Log.writeVerbose("onJoinGuild", `Joined server: ${ guild.name } [ID: ${ guild.id }]`);
  }

  public static onNewMessage(message: Message<boolean>) {
    Log.writeDebug("onNewMessage", `Retrieved new message from ${ message.author.username }#${ message.author.discriminator }.`);
  }

  public static onInvokeInteraction(interaction: Interaction<CacheType>) {
    Log.writeDebug("onInvokeInteraction", `Received interaction from ${ interaction.user.username }#${ interaction.user.discriminator }.`);

    if(interaction.isChatInputCommand()) {
      Log.writeDebug("onInvokeInteraction", "Interaction received is slash command.");
      Handler.onInvokeSlashInteraction(interaction);
    }
  }

  private static onInvokeSlashInteraction(interaction: ChatInputCommandInteraction<CacheType>) {
    Log.writeDebug("onInvokeSlashInteraction", `Received slash interaction from ${ interaction.user.username }#${ interaction.user.discriminator }: ${ interaction.commandName }`);
  }
}
