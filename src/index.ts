// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

import DotEnv from "dotenv";
import { BotClient } from "./client.js";
import { Log } from "./utils/log.js";
import { Environment } from "./utils/env.js";

DotEnv.config({
  debug: process.env.NODE_ENV === "development" || process.env.NODE_ENV === "staging"
});

(async () => {
  Log.writeInfo("index", "Program started.");
  Log.writeInfo("index", "Starting bot client.");

  const botClient = new BotClient(Environment.getBotToken());
  await botClient.run();
})();
