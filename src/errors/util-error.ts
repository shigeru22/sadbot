// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

import { ClientError } from "./client-error.js";

export class UtilError extends ClientError {
  constructor(message?: string) {
    super(message ?? "Unknown utility error occurred.");
  }
}

export class EnvironmentError extends UtilError {
  public environmentKey: string;

  constructor(environmentKey: string, message?: string) {
    super(message ?? "Environment variable error occurred.");
    this.environmentKey = environmentKey;
  }
}
