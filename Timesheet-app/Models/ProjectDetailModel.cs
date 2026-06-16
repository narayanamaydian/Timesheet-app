namespace Timesheet_app.Models
{
    public class ProjectDetailModel
    {
        public string Id { get; set; }
        public string Activities { get; set; }
        public string ProjectId { get; set; } //FK
        public ProjectModel Project { get; set; }
        
    }
}
