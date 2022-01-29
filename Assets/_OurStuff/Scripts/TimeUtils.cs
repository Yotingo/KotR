using System;

public static class TimeUtils
{
    /// <summary>
    /// Returns Unix Time in seconds
    /// </summary>
    public static int GetUnixTime()
    {
        return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
    }

    /// <summary>
    /// Returns current day number
    /// </summary>
    public static int GetCurrentDayNumber()
    {
        return (int)DateTime.Now.DayOfWeek;
    }

    /// <summary>
    /// Returns current day name
    /// </summary>
    public static string GetCurrentDayName()
    {
        return DateTime.Now.DayOfWeek.ToString();
    }

    /// <summary>
    /// Returns current hour and minutes
    /// </summary>
    public static string GetCurrentTime()
    {
        return DateTime.Now.ToString("HH:mm");
    }
}
