using KOLOS1_POPRAWA.Models.DTOs;
using KOLOS1_POPRAWA.Services;
using Microsoft.AspNetCore.Mvc;

namespace KOLOS1_POPRAWA.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtifactsController : ControllerBase
{
    private readonly IDbService _dbService;

    public ArtifactsController(IDbService dbService)
    {
        _dbService = dbService;
    }

    [HttpPost]
    public async Task<IActionResult> AddArtifactAndProject([FromBody] CreateArtifactWithProjectDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _dbService.AddArtifactAndProjectAsync(dto);
            return StatusCode(201, "Successfully created artifact and project.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}