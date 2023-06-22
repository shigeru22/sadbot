// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SadBot.Utils;

public enum LogSeverity
{
	Critical = 0,
	Error,
	Warning,
	Info,
	Verbose,
	Debug
}

public static class Log
{
	private static readonly string[] logSeverityString = {
		"CRITICAL",
		"ERROR",
		"WARNING",
		"INFO",
		"VERBOSE",
		"DEBUG"
	};

	public static void Write(LogSeverity severity, string message, [CallerMemberName] string source = "")
	{
		switch (severity)
		{
			case LogSeverity.Critical:
				WriteCritical(message, source);
				break;
			case LogSeverity.Error:
				WriteError(message, source);
				break;
			case LogSeverity.Warning:
				WriteWarning(message, source);
				break;
			case LogSeverity.Info:
				WriteInfo(message, source);
				break;
			case LogSeverity.Verbose:
				WriteVerbose(message, source);
				break;
			case LogSeverity.Debug:
				WriteDebug(message, source);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
		}
	}

	public static Task WriteAsync(LogSeverity severity, string message, [CallerMemberName] string source = "")
	{
		Write(severity, message, source);
		return Task.CompletedTask;
	}

	public static void WriteCritical(string message, [CallerMemberName] string source = "")
	{
		ConsoleColor currentColor = Console.ForegroundColor;
		Console.ForegroundColor = ConsoleColor.Red;

		Console.WriteLine(GenerateLogMessage(LogSeverity.Critical, message, source));

		Console.ForegroundColor = currentColor;
	}

	public static void WriteError(string message, [CallerMemberName] string source = "")
	{
		ConsoleColor currentColor = Console.ForegroundColor;
		Console.ForegroundColor = ConsoleColor.Red;

		Console.WriteLine(GenerateLogMessage(LogSeverity.Error, message, source));

		Console.ForegroundColor = currentColor;
	}

	public static void WriteWarning(string message, [CallerMemberName] string source = "")
	{
		ConsoleColor currentColor = Console.ForegroundColor;
		Console.ForegroundColor = ConsoleColor.Yellow;

		Console.WriteLine(GenerateLogMessage(LogSeverity.Warning, message, source));

		Console.ForegroundColor = currentColor;
	}

	public static void WriteInfo(string message, [CallerMemberName] string source = "")
	{
		Console.WriteLine(GenerateLogMessage(LogSeverity.Info, message, source));
	}

	public static void WriteVerbose(string message, [CallerMemberName] string source = "")
	{
		ConsoleColor currentColor = Console.ForegroundColor;
		Console.ForegroundColor = ConsoleColor.DarkGray;

		Console.WriteLine(GenerateLogMessage(LogSeverity.Verbose, message, source));

		Console.ForegroundColor = currentColor;
	}

	public static void WriteDebug(string message, [CallerMemberName] string source = "")
	{
		ConsoleColor currentColor = Console.ForegroundColor;
		Console.ForegroundColor = ConsoleColor.DarkGray;

		Console.WriteLine(GenerateLogMessage(LogSeverity.Debug, message, source));

		Console.ForegroundColor = currentColor;
	}

	public static void DeletePreviousLine(bool keepCurrentLine = false)
	{
		int currentCursorLine = Console.CursorTop;
		Console.SetCursorPosition(0, currentCursorLine - 1);
		Console.Write(new string(' ', Console.WindowWidth));

		if (!keepCurrentLine)
		{
			Console.SetCursorPosition(0, currentCursorLine - 1);
		}
	}

	public static string GenerateLogMessage(LogSeverity severity, string message, string source)
	{
		string tempSource = source.Equals(".ctor") || source.Equals(".cctor") ? GetParentTypeName() : source;
		return $"{Date.GetCurrentDateTime()} :: {logSeverityString[(int)severity][0]} :: {tempSource} :: {message}";
	}

	public static string GenerateExceptionMessage(Exception e, string errorMessage)
	{
		return $"{errorMessage}. Exception details below.\n{e}";
	}

	private static string GetParentTypeName()
	{
		StackFrame stackFrame = new StackFrame(3);
		MethodBase? methodBase = stackFrame.GetMethod();

		if (methodBase != null && methodBase.DeclaringType != null)
		{
			return methodBase.DeclaringType.Name;
		}

		return "(unknown)";
	}
}
