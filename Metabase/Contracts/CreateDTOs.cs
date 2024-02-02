using System.Data;

namespace Metabase.Contracts
{
    public record MetaBaseResponse
    {
        public List<Dictionary<string, object>> Data { get; set; }
    }
    public record CreateRelationRequestDTO
    {
        public string RelationName { get; set; }
        public List<CreateAttributeRequestDTO> Attributes { get; set; }
        public List<CreateForeignKeyConstraintRequestDTO> ForeignKeyConstraints { get; set; } = new();
    }

    public record CreateAttributeRequestDTO
    {
        public string RelationName { get; set; } = string.Empty;
        public string AttributeName { get; set; }
        public SqlDbType SqlDbType { get; set; }
        public int? Length {  get; set; }
        public string? DefaultValue { get; set; }
        public bool NotNull { get; set; } = false;
        public bool Unique { get; set; } = false;
        public bool PrimaryKey { get; set; } = false;
    }

    public record CreateDefaultConstraintRequestDTO
    {
        public string RelationName { get; set; }
        public string AttributeName { get; set; }
        public byte[] Value { get; set; }
    }

    public record CreateForeignKeyConstraintRequestDTO
    {
        public string ReferencedTableName { get; set; }
        public string ReferencingTableName { get; set; }
        public List<FKRefrenceDTO> AttributeReferences { get; set; }

    }

    public record FKRefrenceDTO
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}
