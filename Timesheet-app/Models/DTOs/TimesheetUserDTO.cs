using Timesheet_app.Models.DAO;

namespace Timesheet_app.Models.DTOs
{
    public class TimesheetUserDTO
    {
        public required List<TimesheetDTO> Timesheet { get; set; }
        public required UserDto User { get; set; }
    }
}
