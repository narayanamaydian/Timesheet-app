namespace Timesheet_app.Models.DTOs
{
    public class RequestDTO
    {
        public class ClockIn
        {
            public int status { get; set; }
        }

        public class ClockOut
        {
            public string activity { get; set; } 
        }
    }
}
