
namespace Timesheet_app.Models
{
    public class TimesheetModel
    {
        public required string Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly? ClockIn { get; set; }  // Stores exact clock-in time
        public TimeOnly? ClockOut { get; set; } // Stores exact clock-out time
        public TimeSpan AccumulatedTime { get; set; } // Stores total time worked
        public bool Working { get; set; }
        public WorkStatus WFO { get; set; } = WorkStatus.WFH;

        public string UserID { get; set; } // Foreign key to UserModel
        public UserModel User { get; set; } // Navigation property to UserModel
    }

    public enum WorkStatus
    {
        NotWorking = 0,
        WFO = 1,
        WFH = 2 
    }
}
