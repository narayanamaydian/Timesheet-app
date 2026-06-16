namespace Timesheet_app.Models
{
    public class UserProjectModel
    {
        public string Id { get; set; }

        public string UserId { get; set; }
        public UserModel User { get; set; }

        public string ProjectId { get; set; }
        public ProjectModel Project { get; set; }


    }
}
