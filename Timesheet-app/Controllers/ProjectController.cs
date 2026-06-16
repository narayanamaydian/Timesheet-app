using Microsoft.AspNetCore.Mvc;
using Timesheet_app.Models.DTOs;
using Timesheet_app.Services;

namespace Timesheet_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : Controller
    {

        private readonly IProjectService _projectService;
        private readonly IUserProjectService _userProjectService;
        public ProjectController(IProjectService projectService, IUserProjectService userProjectService)
        {
            _projectService = projectService;
            _userProjectService = userProjectService;
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDTO>> AddProject([FromBody] ProjectDTO projectDto)
        {
            if (projectDto == null)
            {
                return BadRequest("Project data is required");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _projectService.AddProjectAsync(projectDto);
                return CreatedAtAction(nameof(AddProject), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("{projectId}")]
        public async Task<ActionResult<ProjectDTO>> GetProject(string projectId)
        {
            try
            {
                var project = await _projectService.GetProjectByIdAsync(projectId);

                if (project == null)
                {
                    return NotFound($"Project with ID {projectId} not found");
                }

                return Ok(project);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("{userId}")]
        public async Task<ActionResult> AssignUserToProject(string userId, [FromBody] string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return BadRequest("Project ID is required");
            }
            try
            {
                var project = await _userProjectService.AssignUserToProjectAsync(userId, projectId);
                return project != null ? Ok(project) : NotFound($"Project with ID {projectId} not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
