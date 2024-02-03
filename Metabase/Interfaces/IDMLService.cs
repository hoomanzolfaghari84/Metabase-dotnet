using Metabase.Contracts;

namespace Metabase.Interfaces
{
    public interface IDMLService
    {
        Task ExecuteQuery();
        Task<List<string>> GetAllDatabases(CancellationToken cancellationToken);
        Task<GetDTOs.GetDatabaseResponseDTO> GetDatabaseAsync(int databaseId, CancellationToken cancellationToken = default);
        Task<GetDTOs.GetDatabaseResponseDTO> GetDatabaseByNameAsync(string databaseName, CancellationToken cancellationToken = default);
        Task<GetDTOs.GetTableResponseDTO> GetRelationAsync(int databaseId, int relationId, CancellationToken cancellationToken = default);
        Task<GetDTOs.GetTableResponseDTO> GetRelationByNameAsync(string databaseName, string relationName, CancellationToken cancellationToken = default);
    }
}