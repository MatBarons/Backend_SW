using System.Globalization;

namespace Academy.Api.Domain.Extensions;

public static class DateExtensions
{
    /// <summary>
    /// Conversion from DateTimeOffset to unix timestamp
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static long? ToTimestamp(this DateTime? date)
    {
        if (date == null)
        {
            return null;
        }

        return new DateTimeOffset((DateTime)date).ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Conversion from unix timestamp to DateTimeOffset
    /// </summary>
    /// <param name="millis"></param>
    /// <returns></returns>
    public static DateTimeOffset? FromTimestamp(this long? millis)
    {
        if (millis == null)
        {
            return null;
        }

        return DateTimeOffset.FromUnixTimeMilliseconds((long)millis);
    }
    
    public static DateTimeOffset TrimToMinutes(this DateTimeOffset dt)
    {
        return new DateTimeOffset(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0, 0, dt.Offset);
    }

    public static DateTimeOffset TrimToHours(this DateTimeOffset dt)
    {
        return new DateTimeOffset(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0, 0, dt.Offset);
    }

    public static DateTimeOffset Min(this DateTimeOffset dt, DateTimeOffset dt2)
    {
        return dt < dt2 ? dt : dt2;
    }

    public static DateTimeOffset Max(this DateTimeOffset dt, DateTimeOffset dt2)
    {
        return dt > dt2 ? dt : dt2;
    }

    public static DateTime TrimToMinutes(this DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0, 0);
    }

    public static DateTime TrimToHours(this DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0, 0);
    }

    public static DateTime Min(this DateTime dt, DateTime dt2)
    {
        return dt < dt2 ? dt : dt2;
    }

    public static DateTime Max(this DateTime dt, DateTime dt2)
    {
        return dt > dt2 ? dt : dt2;
    }
        
    /// <summary>
    /// Gets the TimeZoneInfo.AdjustmentRule in effect for the given year.
    /// </summary>
    /// <param name="timeZoneInfo"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    public static TimeZoneInfo.AdjustmentRule GetAdjustmentRuleForYear(this TimeZoneInfo timeZoneInfo, int year)
    {
        TimeZoneInfo.AdjustmentRule[] adjustments = timeZoneInfo.GetAdjustmentRules();
        // Iterate adjustment rules for time zone 
        foreach (TimeZoneInfo.AdjustmentRule adjustment in adjustments)
        {
            // Determine if this adjustment rule covers year desired 
            if (adjustment.DateStart.Year <= year && adjustment.DateEnd.Year >= year)
            {
                return adjustment;
            }
        }
        return null;
    }
    
    /// <summary>
    /// Gets the Daylight Savings Time start date for the given year.
    /// </summary>
    /// <param name="adjustmentRule"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    public static DateTime GetDaylightTransitionStartForYear(this TimeZoneInfo.AdjustmentRule adjustmentRule, int year)
    {
        return adjustmentRule.DaylightTransitionStart.GetDateForYear(year);
    }
    
    /// <summary>
    /// Gets the Daylight Savings Time end date for the given year.
    /// </summary>
    /// <param name="adjustmentRule"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    public static DateTime GetDaylightTransitionEndForYear(this TimeZoneInfo.AdjustmentRule adjustmentRule, int year)
    {
        return adjustmentRule.DaylightTransitionEnd.GetDateForYear(year);
    }
    
    /// <summary>
    /// Gets the date of the transition for the given year.
    /// </summary>
    /// <param name="transitionTime"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    public static DateTime GetDateForYear(this TimeZoneInfo.TransitionTime transitionTime, int year)
    {
        if (transitionTime.IsFixedDateRule)
        {
            return GetFixedDateRuleDate(transitionTime, year);
        }
        else
        {
            return GetFloatingDateRuleDate(transitionTime, year);
        }
    }

    public static bool IsDSTStartDate(this TimeZoneInfo.AdjustmentRule ar, DateTime d)
    {
        return ar.GetDaylightTransitionStartForYear(d.Year).Date.Equals(d.Date);
    }
        
    public static bool IsDSTEndDate(this TimeZoneInfo.AdjustmentRule ar, DateTime d)
    {
        return ar.GetDaylightTransitionEndForYear(d.Year).Date.Equals(d.Date);
    }
    
    private static DateTime GetFixedDateRuleDate(TimeZoneInfo.TransitionTime transitionTime, int year)
    {
        return new DateTime(year,
            transitionTime.Month,
            transitionTime.Day,
            transitionTime.TimeOfDay.Hour,
            transitionTime.TimeOfDay.Minute,
            transitionTime.TimeOfDay.Second,
            DateTimeKind.Unspecified);
    }
    
    private static DateTime GetFloatingDateRuleDate(TimeZoneInfo.TransitionTime transitionTime, int year)
    {
        // For non-fixed date rules, get local calendar
        Calendar localCalendar = CultureInfo.CurrentCulture.Calendar;
    
        // Get first day of week for transition
        // For example, the 3rd week starts no earlier than the 15th of the month
        int startOfWeek = transitionTime.Week * 7 - 6;
    
        // What day of the week does the month start on?
        int firstDayOfWeek = (int)localCalendar.GetDayOfWeek(new DateTime(year, transitionTime.Month, 1));
    
        // Determine how much start date has to be adjusted
        int transitionDay;
        int changeDayOfWeek = (int)transitionTime.DayOfWeek;
        if (firstDayOfWeek <= changeDayOfWeek)
            transitionDay = startOfWeek + (changeDayOfWeek - firstDayOfWeek);
        else
            transitionDay = startOfWeek + (7 - firstDayOfWeek + changeDayOfWeek);
    
        // Adjust for months with no fifth week
        if (transitionDay > localCalendar.GetDaysInMonth(year, transitionTime.Month))
            transitionDay -= 7;
    
        return new DateTime(year, 
            transitionTime.Month, 
            transitionDay,
            transitionTime.TimeOfDay.Hour,
            transitionTime.TimeOfDay.Minute,
            transitionTime.TimeOfDay.Second,
            DateTimeKind.Unspecified);
    }

    public static int GetTimezone(this DateTime d)
    {
        return (int)(new DateTimeOffset(d)).Offset.TotalHours;
    }

    public static DateTimeOffset AddSecondsDST(this DateTimeOffset d, double s)
    {
        return d.ToUniversalTime().AddSeconds(s).ToLocalTime();
    }

    public static DateTimeOffset AddMinutesDST(this DateTimeOffset d, double m)
    {
        return d.ToUniversalTime().AddMinutes(m).ToLocalTime();
    }

    public static DateTimeOffset AddHoursDST(this DateTimeOffset d, double h)
    {
        return d.ToUniversalTime().AddHours(h).ToLocalTime();
    }

    public static DateTimeOffset AddDaysDST(this DateTimeOffset d, double dd)
    {
        var newDate = d.ToUniversalTime().AddDays(dd).ToLocalTime();
        if (d.Offset != newDate.Offset)
        {//è avvenuto un cambio d'ora
            newDate = newDate.Add(d.Offset - newDate.Offset);
        }

        return newDate;
    }

    public static DateTime AddSecondsDST(this DateTime d, double s)
    {
        return d.ToUniversalTime().AddSeconds(s).ToLocalTime();
    }

    public static DateTime AddMinutesDST(this DateTime d, double m)
    {
        return d.ToUniversalTime().AddMinutes(m).ToLocalTime();
    }

    public static DateTime AddHoursDST(this DateTime d, double h)
    {
        return d.ToUniversalTime().AddHours(h).ToLocalTime();
    }

    public static DateTime AddDaysDST(this DateTime d, double dd)
    {
        return ((DateTimeOffset) d).AddDaysDST(dd).DateTime;
    }
}