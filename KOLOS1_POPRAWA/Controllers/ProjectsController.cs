using KOLOS1_POPRAWA.Services;
using Microsoft.AspNetCore.Mvc;

namespace KOLOS1_POPRAWA.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IDbService _dbService;

    public ProjectsController(IDbService dbService)
    {
        _dbService = dbService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProjectDetails(int id)
    {
        var project = await _dbService.GetProjectDetailsByIdAsync(id);

        if (project == null)
        {
            return NotFound($"Project with id {id} not found.");
        }

        return Ok(project);
    }
}