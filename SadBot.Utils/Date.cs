// Copyright (c) shigeru22. Licensed under the MIT license.
// See LICENSE in the repository root for details.

namespace SadBot.Utils;

public static class Date
{
	private static readonly DateTime zeroTime = new DateTime(1, 1, 1);

	public static string GetCurrentDate(bool useUtc = false)
	{
		DateTime currentTime = useUtc ? DateTime.UtcNow : DateTime.Now;
		return $"{currentTime.Year}/{currentTime.Month.ToString().PadLeft(2, '0')}/{currentTime.Day.ToString().PadLeft(2, '0')}";
	}

	public static string GetCurrentTime(bool useUtc = false)
	{
		DateTime currentTime = useUtc ? DateTime.UtcNow : DateTime.Now;
		return $"{currentTime.Hour.ToString().PadLeft(2, '0')}:{currentTime.Minute.ToString().PadLeft(2, '0')}/{currentTime.Second.ToString().PadLeft(2, '0')}";
	}

	public static string GetCurrentDateTime(bool useUtc = false)
	{
		DateTime currentTime = useUtc ? DateTime.UtcNow : DateTime.Now;
		return $"{currentTime.Year}/{currentTime.Month.ToString().PadLeft(2, '0')}/{currentTime.Day.ToString().PadLeft(2, '0')} {currentTime.Hour.ToString().PadLeft(2, '0')}:{currentTime.Minute.ToString().PadLeft(2, '0')}:{currentTime.Second.ToString().PadLeft(2, '0')}";
	}

	public static string DateTimeToString(DateTime time, bool useUtc = true)
	{
		DateTime tempTime = useUtc ? time.ToUniversalTime() : time;

		// yyyy/mm/dd, hh:mm
		return $"{tempTime.Month}/{tempTime.Day}/{tempTime.Year}, {tempTime.Hour.ToString().PadLeft(2, '0')}:{tempTime.Minute.ToString().PadLeft(2, '0')} ({(useUtc ? "UTC" : "local time")})";
	}

	public static string DeltaTimeToString(TimeSpan time)
	{
		DateTime deltaTime = zeroTime - time;

		if (deltaTime.Year - 1 >= 1)
		{
			return $"{deltaTime.Year - 1} year{(deltaTime.Year - 1 != 1 ? "s" : string.Empty)}";
		}

		if (deltaTime.Month - 1 >= 1)
		{
			return $"{deltaTime.Month} month{(deltaTime.Month - 1 != 1 ? "s" : string.Empty)}";
		}

		if (deltaTime.DayOfYear - 1 >= 1)
		{
			return $"{deltaTime.DayOfYear} day{(deltaTime.DayOfYear - 1 != 1 ? "s" : string.Empty)}";
		}

		if (deltaTime.Hour - 1 >= 1)
		{
			return $"{deltaTime.Hour - 1} hour{(deltaTime.Hour - 1 != 1 ? "s" : string.Empty)}";
		}

		if (deltaTime.Minute >= 1)
		{
			return $"{deltaTime.Minute - 1} minute{(deltaTime.Minute - 1 != 1 ? "s" : string.Empty)}";
		}

		return $"{deltaTime.Second - 1} second{(deltaTime.Second - 1 != 1 ? "s" : string.Empty)}";
	}
}
