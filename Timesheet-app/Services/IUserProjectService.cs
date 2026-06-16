using Timesheet_app.Models.DTOs;

namespace Timesheet_app.Services
{
    public interface IUserProjectService
    {
        public Task<ProjectDTO> AssignUserToProjectAsync(string userId, string projectId);
            public Task<List<ProjectDTO>> GetProjectsByUserIdAsync(string userId);
    }
}
