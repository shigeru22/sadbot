// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

export enum NodeEnvironment {
  DEVELOPMENT,
  STAGING,
  PRODUCTION
}

export class Environment {
  private static nodeEnv: NodeEnvironment | null = null;

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
}
