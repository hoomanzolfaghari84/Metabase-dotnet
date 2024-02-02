using Metabase.Contracts;
using Metabase.Interfaces;
using Metabase.Models;
using Metabase.Models.Attributes;
using Metabase.Models.Constraints;
using Metabase.Persistence;
using Metabase.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Xml.Linq;

namespace Metabase.Services
{
    public class DDLService : IDDLService
    {
        private readonly MetaDBContext _metaContext;
        private readonly ILogger<DDLService> _logger;

        public DDLService(MetaDBContext metaContext, ILogger<DDLService> logger)
        {
            _metaContext = metaContext;
            _logger = logger;
        }


        public async Task<CreateDatabaaseResponseDTO> CreateDatabaseAsync(string databaseName, CancellationToken cancellationToken = default)
        {
            DatabaseModel databaseModel = new()
            {
                Name = databaseName,
            };

            await _metaContext.Databases.AddAsync(databaseModel, cancellationToken);


            var sql = $"CREATE DATABASE {databaseName};";
            _logger.LogWarning("Executing raw sql : " + sql);
            await _metaContext.Database.ExecuteSqlRawAsync(sql, cancellationToken: cancellationToken);

            await _metaContext.SaveChangesAsync(cancellationToken);


            //using DatabasesContext context = new($"Data Source=HOOMAN-LAPTOP\\SQLEXPRESS;Initial Catalog={databaseName};Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");


            //int numberOfRowsAffected = await _metaContext.Database.ExecuteSqlAsync($"CREATE DATABASE {databaseName}", cancellationToken);

            return new CreateDatabaaseResponseDTO()
            {
                Id = databaseModel.Id,
                Name = databaseModel.Name,
            };
        }

        public async Task<CreateRelationResponseDTO> CreateRelationAsync(int databaseId, CreateRelationRequestDTO requestDTO, CancellationToken cancellationToken = default)
        {
            //throw new NotImplementedException();
            DatabaseModel db = await _metaContext.Databases.SingleOrDefaultAsync(d => d.Id == databaseId, cancellationToken: cancellationToken) ?? throw new Exception("database not found");

            if (await _metaContext.Relations.AnyAsync(r => r.Database.Id == db.Id && r.Name == requestDTO.RelationName)) throw new Exception("table name used");

            RelationModel relation = new()
            {
                Database = db,
                Name = requestDTO.RelationName,

            };

            List<AttributeModel> attributes = new();
            List<DefaultConstraint> defaultConstraints = new();

            foreach (var dto in requestDTO.Attributes)
            {
                if (dto.PrimaryKey && (dto.Unique || dto.NotNull)) throw new Exception("pk is already notnull and unique");

                AttributeModel attribute = new AttributeModel()
                {
                    Name = dto.AttributeName,
                    NotNull = dto.NotNull,
                    Unique = dto.Unique,
                    PrimaryKey = dto.PrimaryKey,
                    Relation = relation,
                    Type = dto.SqlDbType,
                    Length = dto.Length,
                };
                attributes.Add(attribute);

                if (dto.DefaultValue is not null)
                {
                    defaultConstraints.Add(new DefaultConstraint()
                    {
                        Attribute = attribute,
                        Type = dto.SqlDbType,
                        Value = SQLTypeHelpers.ConvertSQLStringValueToByteArray(dto.DefaultValue, dto.SqlDbType),
                    });

                }
            }

            relation.Attributes = attributes;


            var attNames = attributes.Select(a => a.Name).ToList();

            var fks = new List<Models.Constraints.ForeignKeyConstraint>();

            foreach (var fk in requestDTO.ForeignKeyConstraints)
            {


                var refedRelation = await _metaContext.Relations
                                                        .Where(r => r.Database.Id == databaseId && r.Name == fk.ReferencedTableName)
                                                        .SingleOrDefaultAsync(cancellationToken) ?? throw new Exception("referenced relation does not exist");

                Models.Constraints.ForeignKeyConstraint foreignKeyConstraint = new()
                {
                    ReferencedRelation = refedRelation,
                    Relation = relation,

                };

                var refedKs = await _metaContext.Attributes.Where(a => a.Relation.Id == refedRelation.Id && (a.PrimaryKey || a.Unique)).ToListAsync(cancellationToken);
                bool hasWholleKey = HasWholeKey(fk, refedKs);

                var refs = new List<FKReference>();

                if (fk.AttributeReferences.Count == 0) throw new Exception();

                foreach (var tuple in fk.AttributeReferences)
                {
                    if (!attNames.Contains(tuple.From)) throw new Exception("att name not in from");

                    refs.Add(new FKReference()
                    {
                        ForeignKeyConstraint = foreignKeyConstraint,
                        ReferencedAttribute = refedKs.Single(a => a.Name == tuple.To),
                        ReferencingAttribute = attributes.Single(a => a.Name == tuple.From),
                    });
                }

                foreignKeyConstraint.References = refs;

                fks.Add(foreignKeyConstraint);

            }


            await _metaContext.AddAsync(relation, cancellationToken);
            await _metaContext.AddRangeAsync(fks, cancellationToken);
            await _metaContext.AddRangeAsync(defaultConstraints, cancellationToken);


            using DatabasesContext context = new($"Data Source=HOOMAN-LAPTOP\\SQLEXPRESS;Initial Catalog={db.Name};Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

            string createScript = GenerateCreateTableScript(relation, defaultConstraints, fks);
            _logger.LogInformation("Executing Raw Sql :" + createScript);
            await context.Database.ExecuteSqlRawAsync(createScript, cancellationToken);

            await _metaContext.SaveChangesAsync(cancellationToken);

            return new CreateRelationResponseDTO()
            {
                Id = relation.Id,
                Name = relation.Name,
                DatabaseName = relation.Database.Name,
                AttributeNames = relation.Attributes.Select(a => a.Name).ToList(),

            };
        }
        private string GenerateCreateTableScript(RelationModel relationModel, List<DefaultConstraint> defaultConstraints, List<Models.Constraints.ForeignKeyConstraint> fks)
        {
            string script = $"CREATE TABLE {relationModel.Name} ( ";
            
            foreach (var attribute in relationModel.Attributes)
            {
                script += $"{attribute.Name} {attribute.Type}";

                if (attribute.Length is not null)
                {
                    if (attribute.Length == -1)
                        script += "(max) ";
                    else
                        script += $"({attribute.Length}) ";

                }
                else script += " ";

                if (attribute.PrimaryKey) script += "PRIMARY KEY ";
                else
                {
                    if (attribute.NotNull) script += "NOT NULL ";
                    if (attribute.Unique) script += "UNIQUE ";
                }

                var dc = defaultConstraints.Where(c => c.Attribute.Id == attribute.Id).SingleOrDefault();
                if (dc is not null)
                {
                    script += $"DEFAULT {dc.Type.GetSqlValueLiteral(dc.Value)} ";
                }

                script += ",";
            }

            foreach (var fk in fks)
            {
                script += "FOREIGN KEY (";
                string referencePart = "(";
                foreach (var reference in fk.References)
                {
                    script += $"{reference.ReferencingAttribute.Name} ,";
                    referencePart += $"{reference.ReferencedAttribute.Name} ,";
                }
                script = script.Remove(script.Length - 1);
                script += ") ";
                referencePart = referencePart.Remove(referencePart.Length - 1);
                referencePart += ")";

                script += $"REFERENCES {fk.ReferencedRelation.Name} {referencePart} ,";

            }

            script = script.Remove(script.Length - 1);
            script += ");";

            return script;
        }
        private static bool HasWholeKey(CreateForeignKeyConstraintRequestDTO fk, List<AttributeModel> refedKs)
        {
            var pk = refedKs.Where(a => a.PrimaryKey).Select(k => k.Name).ToList();

            var fkToNames = fk.AttributeReferences.Select(b => b.To).ToList();

            return pk.All(a => fkToNames.Contains(a));
        }

        public async Task<CreateAttributeResponseDTO> CreateAttributeAsync(int databaseId, int relationId, CreateAttributeRequestDTO requestDTO, CancellationToken cancellationToken = default)
        {
            if (requestDTO.PrimaryKey && (requestDTO.Unique || requestDTO.NotNull)) throw new Exception("pk is already notnull and unique");


            var relation = await _metaContext.Relations.Where(r => r.Id == relationId && r.Database.Id == databaseId).SingleOrDefaultAsync(cancellationToken) ??
                throw new Exception("relation not found");

            if (await _metaContext.Attributes.AnyAsync(a => a.Name == requestDTO.AttributeName && a.Relation.Id == relationId && a.Relation.Database.Id == databaseId))
                throw new Exception("this attribute name already exists in this table");

            //if its a new pk drop old one and corresponding fks

            string script = $"ALTER TABLE {relation.Name} ADD ";

            AttributeModel attributeModel = new()
            {
                Relation = relation,
                Name = requestDTO.AttributeName,
                NotNull = requestDTO.NotNull,
                Unique = requestDTO.Unique,
                PrimaryKey = requestDTO.PrimaryKey,
                Type = requestDTO.SqlDbType,
                Length = requestDTO.Length,
            };

            script += $"{attributeModel.Name} {attributeModel.Type}";

            if (attributeModel.Length is not null)
            {
                if (attributeModel.Length == -1)
                    script += "(max) ";
                else
                    script += $"({attributeModel.Length}) ";

            }
            else script += " ";

            if (attributeModel.PrimaryKey) script += "PRIMARY KEY ";
            else
            {
                if (attributeModel.NotNull) script += "NOT NULL ";
                if (attributeModel.Unique) script += "UNIQUE ";
            }


            if (requestDTO.DefaultValue is not null)
            {
                DefaultConstraint defaultConstraint = new()
                {
                    Attribute = attributeModel,
                    Type = requestDTO.SqlDbType,
                    Value = SQLTypeHelpers.ConvertSQLStringValueToByteArray(requestDTO.DefaultValue, requestDTO.SqlDbType),
                };

                script += $"DEFAULT {defaultConstraint.Type.GetSqlValueLiteral(defaultConstraint.Value)} ";

                await _metaContext.DefaultConstraints.AddAsync(defaultConstraint, cancellationToken);

                
            }

            await _metaContext.Attributes.AddAsync(attributeModel, cancellationToken);

            string dbname = await _metaContext.Databases.Where(d => d.Id == databaseId).Select(d => d.Name).SingleOrDefaultAsync() ?? throw new Exception();

            using DatabasesContext context = new($"Data Source=HOOMAN-LAPTOP\\SQLEXPRESS;Initial Catalog={dbname};Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
            _logger.LogInformation("Executing Raw Sql :" + script);
            await context.Database.ExecuteSqlRawAsync(script, cancellationToken);

            await _metaContext.SaveChangesAsync(cancellationToken);

            return new CreateAttributeResponseDTO()
            {
                Id = attributeModel.Id,
                Name = attributeModel.Name,
                RelationName = attributeModel.Relation.Name,
                Type = requestDTO.SqlDbType.ToString(),
                Length = requestDTO.Length,
                NotNull = attributeModel.NotNull,
                PrimaryKey = attributeModel.PrimaryKey,
                Unique = attributeModel.Unique,
            };
        }

        public async Task<CreateForeignKeyResponseDTO> CreateForeginKeyConstraint(int databaseId, int relationId, CreateForeignKeyConstraintRequestDTO requestDTO, CancellationToken cancellationToken = default)
        {
            var relation = await _metaContext.Relations.Where(r => r.Id == relationId && r.Database.Id == databaseId)
                .Include(r => r.Attributes)
                .SingleOrDefaultAsync(cancellationToken) ??
                throw new Exception("relation not found");

            var refedRelation = await _metaContext.Relations.Where(r => r.Name == requestDTO.ReferencedTableName && r.Database.Id == databaseId).SingleOrDefaultAsync(cancellationToken) ??
                throw new Exception("referenced relation not found");

            Models.Constraints.ForeignKeyConstraint foreignKey = new()
            {
                Relation = relation,
                ReferencedRelation = refedRelation,

            };

            string script = $"ALTER TABLE {relation.Name} ADD FOREIGN KEY (";
            string refscript = "("; 

            var attNames = relation.Attributes.Select(a => a.Name).ToList();

            var refedKs = await _metaContext.Attributes.Where(a => a.Relation.Id == refedRelation.Id && (a.PrimaryKey || a.Unique)).ToListAsync(cancellationToken);
            bool hasWholleKey = HasWholeKey(requestDTO, refedKs);

            var refs = new List<FKReference>();

            if (requestDTO.AttributeReferences.Count == 0) throw new Exception();

            foreach (var tuple in requestDTO.AttributeReferences)
            {
                if (!attNames.Contains(tuple.From)) throw new Exception("att name not in from");

                refs.Add(new FKReference()
                {
                    ForeignKeyConstraint = foreignKey,
                    ReferencedAttribute = refedKs.Single(a => a.Name == tuple.To),
                    ReferencingAttribute = relation.Attributes.Single(a => a.Name == tuple.From),
                });

                script += $"{tuple.From},";
                refscript += $"{tuple.To},";

            }

            script = script.Remove(script.Length - 1);
            refscript = refscript.Remove(refscript.Length - 1);
            script += ")";
            refscript += ")";

            script += $" REFERENCES {refedRelation.Name}" + refscript;

            foreignKey.References = refs;

            await _metaContext.ForeignKeyConstraints.AddAsync(foreignKey, cancellationToken);

            string dbname = await _metaContext.Databases.Where(d => d.Id == databaseId).Select(d => d.Name).SingleOrDefaultAsync() ?? throw new Exception();

            using DatabasesContext context = new($"Data Source=HOOMAN-LAPTOP\\SQLEXPRESS;Initial Catalog={dbname};Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
            _logger.LogInformation("Executing Raw Sql :" + script);
            await context.Database.ExecuteSqlRawAsync(script, cancellationToken);

            await _metaContext.SaveChangesAsync(cancellationToken);

            return new CreateForeignKeyResponseDTO()
            {
                Id = foreignKey.Id,
                ReferencedRelationId = refedRelation.Id,
                ReferencedRelationName = refedRelation.Name,
                ReferencingRelationId = relation.Id,
                ReferencingRelationName = relation.Name,
                References = foreignKey.References.Select(r=> new FKRefrenceDTO() { From = r.ReferencingAttribute.Name, To = r.ReferencedAttribute.Name }).ToList(),
            };
        }



        

    }
}
