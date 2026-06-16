namespace Timesheet_app.Models
{
    public class ProjectModel
    {
        public string Id { get; set; }
        public string ProjectName { get; set; }
        public List<ProjectDetailModel>? Details { get; set; }
        public List<UserProjectModel>? UserProjects { get; set; }
    }
}
