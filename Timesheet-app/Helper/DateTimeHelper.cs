namespace Timesheet_app.Helper
{
    public class DateTimeHelper
    {
        public static DateTime NowJakarta()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
            );
        }

    }
}
