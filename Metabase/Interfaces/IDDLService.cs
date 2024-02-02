using Metabase.Contracts;
using Metabase.Models;
using Metabase.Models.Attributes;
using Metabase.Models.Constraints;

namespace Metabase.Interfaces
{
    public interface IDDLService
    {
        Task<CreateDatabaaseResponseDTO> CreateDatabaseAsync(string databaseName, CancellationToken cancellationToken = default);
        Task<CreateRelationResponseDTO> CreateRelationAsync(int databaseId , CreateRelationRequestDTO requestDTO, CancellationToken cancellationToken = default);
        Task<CreateAttributeResponseDTO> CreateAttributeAsync(int databaseId, int relationId, CreateAttributeRequestDTO requestDTO, CancellationToken cancellationToken = default);
        Task<CreateForeignKeyResponseDTO> CreateForeginKeyConstraint(int databaseId, int relationId, CreateForeignKeyConstraintRequestDTO requestDTO, CancellationToken cancellationToken = default);
    }
}
