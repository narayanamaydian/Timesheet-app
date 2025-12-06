using MongoDB.Bson;

namespace Timesheet_app.Models
{
    public class TimesheetModel
    {
        public ObjectId Id { get; set; }
        public string User_id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly? ClockIn { get; set; }  // Stores exact clock-in time
        public TimeOnly? ClockOut { get; set; } // Stores exact clock-out time
        public TimeSpan AccumulatedTime { get; set; } // Stores total time worked
        public bool Working { get; set; }

    }
}
