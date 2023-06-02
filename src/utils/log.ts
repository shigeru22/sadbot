// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

import chalk from "chalk";
import { DateUtils } from "./date.js";
import { Environment, NodeEnvironment } from "./env.js";

export enum LogSeverity {
  CRITICAL,
  ERROR,
  WARNING,
  INFO,
  VERBOSE,
  DEBUG
}

const severityCode = ["C", "E", "W", "I", "V", "D"];

export class Log {
  public static write(severity: LogSeverity, source: string, message: string) {
    const logText = this.createLogText(severity, source, message);

    switch(severity) {
      case LogSeverity.CRITICAL: // fallthrough
      case LogSeverity.ERROR:
        console.error(chalk.red(logText));
        break;
      case LogSeverity.WARNING:
        console.warn(chalk.yellow(logText));
        break;
      case LogSeverity.INFO:
        console.log(logText);
        break;
      case LogSeverity.VERBOSE: // fallthrough
      case LogSeverity.DEBUG:
        console.debug(chalk.gray(logText));
        break;
    }
  }

  public static writeCritical(source: string, message: string) {
    if(Environment.getLogSeverity() >= LogSeverity.CRITICAL) {
      this.write(LogSeverity.CRITICAL, source, message);
    }
  }

  public static writeError(source: string, message: string) {
    if(Environment.getLogSeverity() >= LogSeverity.ERROR) {
      this.write(LogSeverity.ERROR, source, message);
    }
  }

  public static writeWarning(source: string, message: string) {
    if(Environment.getLogSeverity() >= LogSeverity.WARNING) {
      this.write(LogSeverity.WARNING, source, message);
    }
  }

  public static writeInfo(source: string, message: string) {
    if(Environment.getLogSeverity() >= LogSeverity.INFO) {
      this.write(LogSeverity.INFO, source, message);
    }
  }

  public static writeVerbose(source: string, message: string) {
    if(Environment.getEnvironment() <= NodeEnvironment.STAGING && Environment.getLogSeverity() >= LogSeverity.VERBOSE) {
      this.write(LogSeverity.VERBOSE, source, message);
    }
  }

  public static writeDebug(source: string, message: string) {
    if(Environment.getEnvironment() <= NodeEnvironment.DEVELOPMENT && Environment.getLogSeverity() >= LogSeverity.DEBUG) {
      this.write(LogSeverity.DEBUG, source, message);
    }
  }

  public static createLogText(severity: LogSeverity, source: string, message: string): string {
    // eslint-disable-next-line operator-linebreak
    return `${ DateUtils.getCurrentDateTimeString(Environment.getShouldUseUtc()) } :: ` +
      `${ severityCode[severity] } :: ${ source } :: ${ message }`;
  }

  public static createExceptionMessage(err: Error, errorMessage: string): string {
    return `${ errorMessage }\n${ err.stack === undefined ? `${ err.name }: ${ err.message }` : err.stack }`;
  }
}
