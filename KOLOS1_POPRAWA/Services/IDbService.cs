using KOLOS1_POPRAWA.Models.DTOs;

namespace KOLOS1_POPRAWA.Services;

public interface IDbService
{
    Task<ProjectDetailsDto?> GetProjectDetailsByIdAsync(int id);
    Task AddArtifactAndProjectAsync(CreateArtifactWithProjectDto dto);
}