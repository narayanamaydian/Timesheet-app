using Microsoft.EntityFrameworkCore;
using Timesheet_app.Data;
using Timesheet_app.Models;

namespace Timesheet_app.Repositories
{
    public class ProjectRepo : IProjectRepo
    {
        private SQLServerDBConnection _dbContext;
        public ProjectRepo(SQLServerDBConnection dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProjectModel> AddProjectModelAsync(ProjectModel ProjectModel)
        {
            try
            {
                await _dbContext.Projects.AddAsync(ProjectModel);
                await _dbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception($"Error adding project: {ex.Message}");
            }

            return ProjectModel;
        }

        public async Task<bool> DeleteProjectModelAsync(string ProjectModelId)
        {
            try
            {
                _ = _dbContext.Projects.Where(p => p.Id == ProjectModelId).ExecuteDeleteAsync();
                await _dbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception($"Error deleting project: {ex.Message}");
            } 
            return true;
        }

        public async Task<List<ProjectModel>> GetAllProjectModelsAsync()
        {
            try
            {
                var projects = await _dbContext.Projects.ToListAsync();
                return projects;
            }
            catch(Exception ex)
            {
                throw new Exception($"Error retrieving projects: {ex.Message}");
            }   
        }

        public Task<ProjectModel> GetProjectModelByIdAsync(string ProjectModelId)
        {
            try
            {
                var project = _dbContext.Projects.Where(p => p.Id == ProjectModelId).FirstOrDefaultAsync();
                if (project == null)
                {
                    throw new Exception("Project not found");
                }
                return project;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving project: {ex.Message}");
            }
                
        }

        public async Task<ProjectModel> UpdateProjectModelAsync(ProjectModel ProjectModel)
        {
            try
            {
                var project = await _dbContext.Projects.Where(p => p.Id == ProjectModel.Id).FirstOrDefaultAsync();  
                if (project == null)
                {
                    throw new Exception("Project not found");
                }
                _dbContext.Projects.Update(ProjectModel);
                await _dbContext.SaveChangesAsync();
                return ProjectModel;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating project: {ex.Message}");
            }
        }
    }
}
