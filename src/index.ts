// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

import DotEnv from "dotenv";
import { Log } from "./utils/log.js";

DotEnv.config({
  debug: process.env.NODE_ENV === "development" || process.env.NODE_ENV === "staging"
});

Log.writeCritical("index", "This is a critical error message.");
Log.writeError("index", "This is an error message.");
Log.writeWarning("index", "This is a warning message.");
Log.writeInfo("index", "This is an info message.");
Log.writeVerbose("index", "This is a verbose message.");
Log.writeDebug("index", "This is a debug message.");

const tempErr = new EvalError("Test error.");
Log.writeError("index", Log.createExceptionMessage(tempErr, "This is an error with exception message."));
