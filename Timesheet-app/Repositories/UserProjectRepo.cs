using Microsoft.EntityFrameworkCore;
using Timesheet_app.Data;
using Timesheet_app.Models;

namespace Timesheet_app.Repositories
{
    public class UserProjectRepo : IUserProjectRepo
    {
        private readonly SQLServerDBConnection _dbContext;
        private readonly IProjectRepo _projectRepo;

        public UserProjectRepo(SQLServerDBConnection dbContext, IProjectRepo projectRepo)
        {
            _dbContext = dbContext;
            _projectRepo = projectRepo;
        }

        public async Task<ProjectModel> AsignUserToProject(string userId, string projectId)
        {
            var userProject = new UserProjectModel
            {
                Id = $"{userId}_{projectId}",
                 UserId = userId,
                ProjectId = projectId
            };

            await _dbContext.UserProjects.AddAsync(userProject);
            await _dbContext.SaveChangesAsync();

            return await _projectRepo.GetProjectModelByIdAsync(projectId);
        }

        public async Task<List<ProjectModel>> GetProjectsByUserId(string userId)
        {
            var project = await _dbContext.UserProjects
                .Where(up => up.UserId == userId)
                .Select(up => up.Project)
                .ToListAsync();

            return project;
        }
    }
}
