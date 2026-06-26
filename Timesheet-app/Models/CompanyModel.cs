namespace Timesheet_app.Models
{
    public class CompanyModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<UserModel> Users { get; set; } // Navigation property to UserModel
        public List<TimesheetTemplateModel> TimesheetTemplates { get; set; } // Navigation property to TimesheetTemplateModel
    }
}
