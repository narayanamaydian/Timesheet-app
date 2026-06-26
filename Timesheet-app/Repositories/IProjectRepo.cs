using Timesheet_app.Models;

namespace Timesheet_app.Repositories
{
    public interface IProjectRepo
    {
            public Task<List<ProjectModel>> GetAllProjectModelsAsync();
            public Task<ProjectModel> GetProjectModelByIdAsync(string ProjectModelId);
            public Task<ProjectModel> AddProjectModelAsync(ProjectModel ProjectModel);
            public Task<ProjectModel> UpdateProjectModelAsync(ProjectModel ProjectModel);
            public Task<bool> DeleteProjectModelAsync(string ProjectModelId);
    }
}
