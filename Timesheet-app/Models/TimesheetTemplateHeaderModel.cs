namespace Timesheet_app.Models
{
    public class TimesheetTemplateHeaderModel
    {
        public string Id { get; set; }
        public string Header { get; set; }
        public string TemplateId { get; set; } // Foreign key to TimesheetTemplateModel
    }
}
