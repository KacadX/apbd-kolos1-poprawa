using System.Data.Common;
using KOLOS1_POPRAWA.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace KOLOS1_POPRAWA.Services;

public class DbService : IDbService
{
    private readonly string _connectionString;

    public DbService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default")!;
    }

    public async Task<ProjectDetailsDto?> GetProjectDetailsByIdAsync(int id)
    {
        var query = @"
            SELECT
                p.ProjectId, p.Objective, p.StartDate, p.EndDate,
                a.Name AS ArtifactName, a.OriginDate,
                i.InstitutionId, i.Name AS InstitutionName, i.FoundedYear,
                s.FirstName, s.LastName, s.HireDate, sa.Role
            FROM Preservation_Project p
            JOIN Artifact a ON p.ArtifactId = a.ArtifactId
            JOIN Institution i ON a.InstitutionId = i.InstitutionId
            LEFT JOIN Staff_Assignment sa ON p.ProjectId = sa.ProjectId
            LEFT JOIN Staff s ON sa.StaffId = s.StaffId
            WHERE p.ProjectId = @ProjectId;";

        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(query, conn);
        
        cmd.Parameters.AddWithValue("@ProjectId", id);
        await conn.OpenAsync();

        ProjectDetailsDto? projectDetailsDto = null;
        var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            if (projectDetailsDto == null)
            {
                projectDetailsDto = new ProjectDetailsDto
                {
                    ProjectId = reader.GetInt32(reader.GetOrdinal("ProjectId")),
                    Objective = reader.GetString(reader.GetOrdinal("Objective")),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = reader.IsDBNull(reader.GetOrdinal("EndDate")) ? null : reader.GetDateTime(reader.GetOrdinal("EndDate")),
                    Artifact = new ArtifactDto
                    {
                        Name = reader.GetString(reader.GetOrdinal("ArtifactName")),
                        OriginDate = reader.GetDateTime(reader.GetOrdinal("OriginDate")),
                        Institution = new InstitutionDto
                        {
                            InstitutionId = reader.GetInt32(reader.GetOrdinal("InstitutionId")),
                            Name = reader.GetString(reader.GetOrdinal("InstitutionName")),
                            FoundedYear = reader.GetInt32(reader.GetOrdinal("FoundedYear"))
                        }
                    }
                };
            }

            if (!reader.IsDBNull(reader.GetOrdinal("Role")))
            {
                projectDetailsDto.StaffAssignments.Add(new StaffAssignmentDto
                {
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    HireDate = reader.GetDateTime(reader.GetOrdinal("HireDate")),
                    Role = reader.GetString(reader.GetOrdinal("Role"))
                });
            }
        }

        return projectDetailsDto;
    }

    public async Task AddArtifactAndProjectAsync(CreateArtifactWithProjectDto dto)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var transaction = (SqlTransaction)await conn.BeginTransactionAsync();
        
        try
        {
            var cmdCheckInstitution = new SqlCommand("SELECT 1 FROM Institution WHERE InstitutionId = @InstitutionId", conn, transaction);
            cmdCheckInstitution.Parameters.AddWithValue("@InstitutionId", dto.Artifact.InstitutionId);
            var institutionExists = await cmdCheckInstitution.ExecuteScalarAsync();
            if (institutionExists == null)
            {
                throw new Exception($"Institution with ID {dto.Artifact.InstitutionId} does not exist.");
            }
            
            var cmdInsertArtifact = new SqlCommand(
                "INSERT INTO Artifact(ArtifactId, Name, OriginDate, InstitutionId) VALUES (@ArtifactId, @Name, @OriginDate, @InstitutionId);",
                conn, transaction);
            cmdInsertArtifact.Parameters.AddWithValue("@ArtifactId", dto.Artifact.ArtifactId);
            cmdInsertArtifact.Parameters.AddWithValue("@Name", dto.Artifact.Name);
            cmdInsertArtifact.Parameters.AddWithValue("@OriginDate", dto.Artifact.OriginDate);
            cmdInsertArtifact.Parameters.AddWithValue("@InstitutionId", dto.Artifact.InstitutionId);
            await cmdInsertArtifact.ExecuteNonQueryAsync();
            
            var cmdInsertProject = new SqlCommand(
                "INSERT INTO Preservation_Project(ProjectId, ArtifactId, StartDate, EndDate, Objective) VALUES (@ProjectId, @ArtifactId, @StartDate, @EndDate, @Objective);",
                conn, transaction);
            cmdInsertProject.Parameters.AddWithValue("@ProjectId", dto.Project.ProjectId);
            cmdInsertProject.Parameters.AddWithValue("@ArtifactId", dto.Artifact.ArtifactId);
            cmdInsertProject.Parameters.AddWithValue("@StartDate", dto.Project.StartDate);
            cmdInsertProject.Parameters.AddWithValue("@EndDate", (object)dto.Project.EndDate ?? DBNull.Value);
            cmdInsertProject.Parameters.AddWithValue("@Objective", dto.Project.Objective);
            await cmdInsertProject.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }
        catch (SqlException ex) when (ex.Number == 2627)
        {
            await transaction.RollbackAsync();
            throw new Exception("Artifact or Project with the given ID already exists.");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}