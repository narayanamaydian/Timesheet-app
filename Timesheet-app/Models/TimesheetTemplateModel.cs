namespace Timesheet_app.Models
{
    public class TimesheetTemplateModel
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public string? FileName { get; set; }
        public string? CompanyId { get; set; } // Foreign key to CompanyModel
        public string? BaseTemplateId { get; set; } // FK to BaseTemplate (null for BaseTemplate)
        public string? UserId { get; set; } // Foreign key to UserModel
        public bool IsBased { get; set; } = true; // Indicates if the template is based on another template
        public List<TimesheetTemplateHeaderModel>? Header { get; set; } // Navigation property to TimesheetTemplateHeaderModel
    }
}
