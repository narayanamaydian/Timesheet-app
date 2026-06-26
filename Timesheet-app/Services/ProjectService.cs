using Timesheet_app.Models;
using Timesheet_app.Models.DTOs;
using Timesheet_app.Repositories;

namespace Timesheet_app.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepo _projectRepo;
        public ProjectService(IProjectRepo projectRepo)
        {
            _projectRepo = projectRepo;
        }

        public async Task<ProjectDTO> AddProjectAsync(ProjectDTO projectDto)
        {
            var date = DateTime.UtcNow;
            var projectModel = new ProjectModel
            {
                Id = $"{projectDto.ProjectName}_{date:yyyyMMdd}",
                ProjectName = projectDto.ProjectName
            };  
            await _projectRepo.AddProjectModelAsync(projectModel);

            return new ProjectDTO
            {
                Id = projectModel.Id,
                ProjectName = projectModel.ProjectName
            };

        }

        public async Task<bool> DeleteProjectAsync(string projectId)
        {
            var project = await _projectRepo.GetProjectModelByIdAsync(projectId);
            if (project == null)
            {
                return false; // Project not found
            }
            await _projectRepo.DeleteProjectModelAsync(projectId);
            return true;
        }

        public async Task<List<ProjectDTO>> GetAllProjectsAsync()
        {
            var projectModels = await _projectRepo.GetAllProjectModelsAsync();
            var projectDtos = projectModels.Select(pm => new ProjectDTO
            {
                Id = pm.Id,
                ProjectName = pm.ProjectName
            }).ToList();
            return projectDtos;
        }

        public async Task<ProjectDTO> GetProjectByIdAsync(string projectId)
        {
            var projectModel = await _projectRepo.GetProjectModelByIdAsync(projectId);
            if (projectModel == null)
            {
                return null; // Project not found
            }
            return new ProjectDTO
            {
                Id = projectModel.Id,
                ProjectName = projectModel.ProjectName
            };
        }
        

        public async Task<ProjectDTO> UpdateProjectAsync(string projectId, ProjectDTO projectDto)
        {
            var projectModel = await _projectRepo.GetProjectModelByIdAsync(projectId);
            if (projectModel == null)
            {
                return null; // Project not found
            }

            projectModel.ProjectName = projectDto.ProjectName;
            await _projectRepo.UpdateProjectModelAsync(projectModel);

            return new ProjectDTO
            {
                Id = projectModel.Id,
                ProjectName = projectModel.ProjectName
            };
        }
    }
}
