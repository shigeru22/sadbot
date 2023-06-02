// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

import { EnvironmentError } from "../errors/util-error.js";
import { LogSeverity } from "./log.js";

export enum NodeEnvironment {
  DEVELOPMENT,
  STAGING,
  PRODUCTION
}

export class Environment {
  private static nodeEnv: NodeEnvironment | null = null;
  private static logSeverity: LogSeverity | null = null;

  public static getEnvironment(): NodeEnvironment {
    if(this.nodeEnv === null) {
      const tempEnv = process.env.NODE_ENV;

      switch(tempEnv) {
        case "development":
          this.nodeEnv = NodeEnvironment.DEVELOPMENT;
          break;
        case "staging":
          this.nodeEnv = NodeEnvironment.STAGING;
          break;
        case "production": // fallthrough
        default:
          this.nodeEnv = NodeEnvironment.PRODUCTION;
      }
    }

    return this.nodeEnv;
  }

  public static getBotToken(): string {
    if(process.env.SB_BOT_TOKEN === undefined || process.env.SB_BOT_TOKEN === "") {
      throw new EnvironmentError("SB_BOT_TOKEN", "SB_BOT_TOKEN is not defined in environment variables.");
    }

    return process.env.SB_BOT_TOKEN;
  }

  public static getShouldUseUtc(): boolean {
    return process.env.SB_LOG_USE_UTC === "1" || process.env.SB_LOG_USE_UTC === "true";
  }

  public static getLogSeverity(): LogSeverity {
    if(this.logSeverity === null) {
      const tempSeverity = process.env.SB_LOG_SEVERITY || "3";

      switch(tempSeverity) {
        case "0":
          this.logSeverity = LogSeverity.CRITICAL;
          break;
        case "1":
          this.logSeverity = LogSeverity.ERROR;
          break;
        case "2":
          this.logSeverity = LogSeverity.WARNING;
          break;
        case "4":
          this.logSeverity = LogSeverity.VERBOSE;
          break;
        case "5":
          this.logSeverity = LogSeverity.DEBUG;
          break;
        case "3": // fallthrough
        default:
          this.logSeverity = LogSeverity.INFO;
      }
    }

    return this.logSeverity;
  }
}
