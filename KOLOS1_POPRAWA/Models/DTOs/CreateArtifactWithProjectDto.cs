using System.ComponentModel.DataAnnotations;

namespace KOLOS1_POPRAWA.Models.DTOs;

public class CreateArtifactWithProjectDto
{
    [Required]
    public NewArtifactDto Artifact { get; set; } = null!;
    [Required]
    public NewProjectDto Project { get; set; } = null!;
}

public class NewArtifactDto
{
    [Required]
    public int ArtifactId { get; set; }
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    [Required]
    public DateTime OriginDate { get; set; }
    [Required]
    public int InstitutionId { get; set; }
}


public class NewProjectDto
{
    [Required]
    public int ProjectId { get; set; }
    [Required]
    [MaxLength(200)]
    public string Objective { get; set; } = string.Empty;
    [Required]
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}