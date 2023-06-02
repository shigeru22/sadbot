// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

/* eslint-disable operator-linebreak */ // disable line-break for current string concatenations

export class DateUtils {
  public static getCurrentDateString(useUtc?: boolean): string {
    const currentTime = new Date();
    let currentMonth: number;

    if(useUtc === true) {
      currentMonth = currentTime.getUTCMonth() + 1;

      return `${ currentTime.getUTCFullYear().toString() }/` +
        `${ currentMonth.toString().padStart(2, "0") }/` +
        `${ currentTime.getUTCDate().toString().padStart(2, "0") }`;
    }

    currentMonth = currentTime.getMonth() + 1;

    return `${ currentTime.getFullYear().toString() }/` +
      `${ currentMonth.toString().padStart(2, "0") }/` +
      `${ currentTime.getDate().toString().padStart(2, "0") }`;
  }

  public static getCurrentTimeString(useUtc?: boolean): string {
    const currentTime = new Date();

    if(useUtc === true) {
      return `${ currentTime.getUTCHours().toString().padStart(2, "0") }:` +
        `${ currentTime.getUTCMinutes().toString().padStart(2, "0") }:` +
        `${ currentTime.getUTCSeconds().toString().padStart(2, "0") }`;
    }

    return `${ currentTime.getHours().toString().padStart(2, "0") }:` +
      `${ currentTime.getMinutes().toString().padStart(2, "0") }:` +
      `${ currentTime.getSeconds().toString().padStart(2, "0") }`;
  }

  public static getCurrentDateTimeString(useUtc?: boolean): string {
    const currentTime = new Date();
    let currentMonth: number;

    if(useUtc === true) {
      currentMonth = currentTime.getUTCMonth() + 1;

      return `${ currentTime.getUTCFullYear().toString() }/` +
        `${ currentMonth.toString().padStart(2, "0") }/` +
        `${ currentTime.getUTCDate().toString().padStart(2, "0") } ` +
        `${ currentTime.getUTCHours().toString().padStart(2, "0") }:` +
        `${ currentTime.getUTCMinutes().toString().padStart(2, "0") }:` +
        `${ currentTime.getUTCSeconds().toString().padStart(2, "0") }`;
    }

    currentMonth = currentTime.getMonth() + 1;

    return `${ currentTime.getFullYear().toString() }/` +
      `${ currentMonth.toString().padStart(2, "0") }/` +
      `${ currentTime.getDate().toString().padStart(2, "0") } ` +
      `${ currentTime.getHours().toString().padStart(2, "0") }:` +
      `${ currentTime.getMinutes().toString().padStart(2, "0") }:` +
      `${ currentTime.getSeconds().toString().padStart(2, "0") }`;
  }
}
