using Timesheet_app.Models.DTOs;

namespace Timesheet_app.Services
{
    public interface IProjectService
    {
        public Task<List<ProjectDTO>> GetAllProjectsAsync();
        public Task<ProjectDTO> GetProjectByIdAsync(string projectId);
        public Task<ProjectDTO> AddProjectAsync(ProjectDTO projectDto);
        public Task<ProjectDTO> UpdateProjectAsync(string projectId, ProjectDTO projectDto);
        public Task<bool> DeleteProjectAsync(string projectId);

    }
}
