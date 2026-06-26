using Timesheet_app.Models;

namespace Timesheet_app.Repositories
{
    public interface IUserProjectRepo
    {
        public Task<ProjectModel> AsignUserToProject(string userId, string projectId);
         public Task<List<ProjectModel>> GetProjectsByUserId(string userId);
    }
}
