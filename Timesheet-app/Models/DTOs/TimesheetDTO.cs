using System.ComponentModel;

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
        public WorkStatus WFO { get; set; } = WorkStatus.WFH;

        public enum WorkStatus
        {
            [Description("Not Working")]
            NotWorking = 0,

            [Description("WFO")]
            WFO = 1,

            [Description("WFH")]
            WFH = 2
        }

    }
}
