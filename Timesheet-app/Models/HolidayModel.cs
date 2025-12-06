using MongoDB.Bson;

namespace Timesheet_app.Models
{
    public class HolidayModel
    {
        public ObjectId Id { get; set; }
        public int Day { get; set; } // Day of the month
        public int Month { get; set; } // Month of the year
        public int? Year { get; set; } // Year of the holiday
        public bool Recurring { get; set; } // Indicates if the holiday recurs every year
    }
}
