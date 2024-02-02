using Metabase.Contracts;
using Metabase.Interfaces;
using Metabase.Models;
using Metabase.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading;
using static Metabase.Contracts.GetDTOs;

namespace Metabase.Services
{
    public class DMLService : IDMLService
    {
        private readonly MetaDBContext _metaContext;
        private readonly ILogger<DMLService> _logger;
        public DMLService(MetaDBContext metaContext, ILogger<DMLService> logger)
        {
            _metaContext = metaContext;
            _logger = logger;
        }

        public async Task<GetDatabaseResponseDTO> GetDatabaseAsync(int databaseId, CancellationToken cancellationToken = default)
        {
            var database = await _metaContext.Databases.Where(d => d.Id == databaseId).Include(d => d.Relations).ThenInclude(t => t.Attributes).SingleOrDefaultAsync() ?? throw new Exception("database not found");
            GetDatabaseResponseDTO response = GetDatabaseDTOFromDatabaseModel(database);

            return response;
        }

        private GetDatabaseResponseDTO GetDatabaseDTOFromDatabaseModel(DatabaseModel database)
        {
            return new GetDatabaseResponseDTO()
            {
                Id = database.Id,
                Name = database.Name,
                Tables = database.Relations.Select(r => new GetTableResponseDTO()
                {
                    Id = r.Id,
                    TableName = r.Name,
                    Attributes = r.Attributes.Select(a => new GetAttributeResponseDTO
                    {
                        Id = a.Id,
                        AttributeName = a.Name,
                        NotNull = a.NotNull,
                        PrimaryKey = a.PrimaryKey,
                        Unique = a.Unique,
                        Type = a.Type.ToString(),
                    }).ToList(),
                    ForeignKeys =
                                [
                                    .. _metaContext.ForeignKeyConstraints.Where(f => f.Relation.Id == r.Id).Select(f => new ForeignKeyResponseDTO()
                                    {
                                        ReferencedTableName = f.ReferencedRelation.Name,
                                        References = f.References.Select(refs => new GetReferenceDTO() { From = refs.ReferencingAttribute.Name, To = refs.ReferencedAttribute.Name }).ToList(),
                                    }),
                                ],
                }).ToList(),

            };
        }

        public async Task<GetDatabaseResponseDTO> GetDatabaseByNameAsync(string databaseName, CancellationToken cancellationToken = default)
        {
            var database = await _metaContext.Databases.Where(d => d.Name == databaseName).Include(d => d.Relations).ThenInclude(t => t.Attributes).SingleOrDefaultAsync() ?? throw new Exception("database not found");
            GetDatabaseResponseDTO response = GetDatabaseDTOFromDatabaseModel(database);

            return response;
        }



        public async Task<GetTableResponseDTO> GetRelationAsync(int databaseId, int relationId, CancellationToken cancellationToken = default)
        {
            var query = _metaContext.Relations.Where(r => r.Id == relationId);
            return await GetTableResponseDTOFromRelationModel(query, cancellationToken);
        }

        private async Task<GetTableResponseDTO> GetTableResponseDTOFromRelationModel(IQueryable<RelationModel> query, CancellationToken cancellationToken = default)
        {
            return await query.Include(r => r.Attributes).Select(r => new GetTableResponseDTO()
            {
                Id = r.Id,
                TableName = r.Name,
                Attributes = r.Attributes.Select(a => new GetAttributeResponseDTO
                {
                    Id = a.Id,
                    AttributeName = a.Name,
                    NotNull = a.NotNull,
                    PrimaryKey = a.PrimaryKey,
                    Unique = a.Unique,
                    Type = a.Type.ToString(),
                }).ToList(),
                ForeignKeys = _metaContext.ForeignKeyConstraints.Where(f => f.Relation.Id == r.Id).Select(f => new ForeignKeyResponseDTO()
                {
                    ReferencedTableName = f.ReferencedRelation.Name,
                    References = f.References.Select(refs => new GetReferenceDTO() { From = refs.ReferencingAttribute.Name, To = refs.ReferencedAttribute.Name }).ToList(),
                }).ToList(),

            }).SingleOrDefaultAsync(cancellationToken) ?? throw new Exception("relation not found");
        }


        public async Task<GetTableResponseDTO> GetRelationByNameAsync(string databaseName, string relationName, CancellationToken cancellationToken = default)
        {
            var query = _metaContext.Relations.Where(r => r.Name == relationName && r.Database.Name == databaseName);
            return await GetTableResponseDTOFromRelationModel(query, cancellationToken);
        }

        public async Task ExecuteQuery()
        {
            throw new NotImplementedException();
        }
    }
}
