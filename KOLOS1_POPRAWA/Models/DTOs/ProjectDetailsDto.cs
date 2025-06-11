namespace KOLOS1_POPRAWA.Models.DTOs;

public class ProjectDetailsDto
{
    public int ProjectId { get; set; }
    public string Objective { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ArtifactDto Artifact { get; set; } = null!;
    public List<StaffAssignmentDto> StaffAssignments { get; set; } = new();
}

public class ArtifactDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime OriginDate { get; set; }
    public InstitutionDto Institution { get; set; } = null!;
}

public class InstitutionDto
{
    public int InstitutionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int FoundedYear { get; set; }
}

public class StaffAssignmentDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public string Role { get; set; } = string.Empty;
}