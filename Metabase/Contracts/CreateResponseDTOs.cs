using System.Data;

namespace Metabase.Contracts
{
    public record CreateDatabaaseResponseDTO()
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public record CreateRelationResponseDTO()
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DatabaseName { get; set; }
        public List<string> AttributeNames { get; set; }
    }

    public record CreateAttributeResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RelationName { get; set; }
        public string Type { get; set; }
        public int? Length { get; set; }
        public bool NotNull { get; set; } = false;
        public bool Unique { get; set; } = false;
        public bool PrimaryKey { get; set; } = false;

    }
    public record CreateForeignKeyResponseDTO()
    {
        public int Id { get; set; }
        public int ReferencingRelationId { get; set; }
        public string ReferencingRelationName { get; set; }
        public int ReferencedRelationId { get; set; }
        public string ReferencedRelationName { get; set; }
        public List<FKRefrenceDTO> References { get; set; }


    }
}
