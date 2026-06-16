

namespace Timesheet_app.Models
{
    public class UserModel
    {
        public required string Id { get; set; }    
        public required string Name { get; set; }
        public required string VendorName { get; set; }
        public required string NoSpk { get; set; }
        public List<TimesheetModel>? Timesheets { get; set; }
        public List<UserProjectModel>? UserProjects { get; set; }


    }
}
