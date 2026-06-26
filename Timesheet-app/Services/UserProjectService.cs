using Timesheet_app.Models.DTOs;
using Timesheet_app.Repositories;

namespace Timesheet_app.Services
{
    public class UserProjectService : IUserProjectService
    {
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private readonly IUserProjectRepo _userProjectRepo;

        public UserProjectService(IUserService userService, IProjectService projectService, IUserProjectRepo userProjectRepo)
        {
            _userService = userService;
            _projectService = projectService;
            _userProjectRepo = userProjectRepo;
        }

        public async Task<ProjectDTO> AssignUserToProjectAsync(string userId, string projectId)
        {
            await _userProjectRepo.AsignUserToProject(userId, projectId);
            return await _projectService.GetProjectByIdAsync(projectId);
        }

        public async Task<List<ProjectDTO>> GetProjectsByUserIdAsync(string userId)
        {
            var projects = await _userProjectRepo.GetProjectsByUserId(userId);
            var newProjects = new List<ProjectDTO>();
            foreach (var project in projects)
            {
                newProjects.Add(new ProjectDTO()
                {
                    ProjectName = project.ProjectName,
                    Id = project.Id
                });
            }
            return newProjects;
        }
    }
}
