namespace IAUI.Agent.Plugins.FunctionPlugin;

using System.ComponentModel;

public class TimePlugin()
{
    [KernelFunction("get_time")]
    [Description("Get the current time.")]
    public string GetTime()
    {
        return DateTime.Now.ToString("HH:mm:ss");
    }

    [KernelFunction("get_date")]
    [Description("Get the current date.")]
    public string GetDate()
    {
        return DateTime.Now.ToString("yyyy-MM-dd");
    }

    [KernelFunction("get_date_with_format")]
    [Description("Get the current date with requested fornmat")]
    public string GetDate(
        [Description(
            "contains the desired date time format in which the output date time should be"
        )]
            string format
    )
    {
        return DateTime.Now.ToString(format);
    }

    [KernelFunction("get_time_and_date")]
    [Description("Get the current time and date.")]
    public string GetTimeAndDate()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    [KernelFunction("get_time_and_date_with_format")]
    [Description("Get the current time and date with requested format.")]
    public string GetTimeAndDate(
        [Description("Desired format in which the current datetime has to be converted to")]
            string format
    )
    {
        return DateTime.Now.ToString(format);
    }

    [KernelFunction("get_local_time_zone")]
    [Description("Get the local time zone.")]
    public string GetLocalTimeZone()
    {
        return TimeZoneInfo.Local.DisplayName;
    }

    [KernelFunction("get_local_time_zone_iana")]
    [Description("Get the local time zone in IANA format.")]
    public string GetLocalTimeZoneIana()
    {
        return TimeZoneInfo.Local.Id;
    }

    [KernelFunction("convert_to_desired_time_zone")]
    [Description("Convert the given time to the desired time zone.")]
    public string ConvertToDesiredTimeZone(
        [Description("date time to be converted")] string time,
        [Description("source datetime time zone")] string sourceTimeZone,
        [Description("desired datetime time zone to which the conversion should happen")]
            string destinationTimeZone
    )
    {
        DateTime dateTime = DateTime.Parse(time);
        TimeZoneInfo sourceTZ = TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZone);
        TimeZoneInfo destinationTZ = TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZone);

        dateTime = TimeZoneInfo.ConvertTime(dateTime, sourceTZ, destinationTZ);
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    [KernelFunction("get_time_in_time_zone")]
    [Description("Get the current time in the given time zone.")]
    public string GetCurrentTimeInTimeZone(
        [Description("Desired timezone in which we want the current time to be")] string timeZone
    )
    {
        DateTime dateTime = DateTime.UtcNow;
        TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
        dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZoneInfo);
        return dateTime.ToString("HH:mm:ss");
    }

    [KernelFunction("get_current_date_time_in_time_zone")]
    [Description("Get the current time in the given time zone.")]
    public string GetCurrentDateTimeInTimeZone(
        [Description("Desired timezone in which we want the current datetime to be")]
            string timeZone
    )
    {
        DateTime dateTime = DateTime.UtcNow;
        TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
        dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZoneInfo);
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    [KernelFunction("Get_Date_Time_In_Date_Time_Offset")]
    [Description("Get the Date time offset for the given Date Time.")]
    public DateTimeOffset GetDateTimeInDateTimeOffset(
        [Description("date time to be converted to date time offset")] string dateTime
    )
    {
        return DateTimeOffset.Parse(dateTime);
    }
}
