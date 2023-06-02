// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

import DotEnv from "dotenv";
import { Environment } from "./utils/env";

DotEnv.config({
  debug: process.env.NODE_ENV === "development" || process.env.NODE_ENV === "staging"
});

console.log(Environment.getEnvironment());
