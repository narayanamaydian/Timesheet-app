namespace Timesheet_app.Models.DAO
{
    public class TimesheetDTO
    {
        public required string Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly? ClockIn { get; set; }  // Stores exact clock-in time
        public TimeOnly? ClockOut { get; set; } // Stores exact clock-out time
        public TimeSpan AccumulatedTime { get; set; } // Stores total time worked
        public bool Working { get; set; }
    }
}
