// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

export class ClientError extends Error {
  constructor(message?: string) {
    super(message ?? "Unknown client error occurred.");
  }
}
