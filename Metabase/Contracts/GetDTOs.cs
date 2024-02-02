namespace Metabase.Contracts
{
    public class GetDTOs
    {
        public record GetDatabaseResponseDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<GetTableResponseDTO> Tables { get; set; }
            
        }

        public record GetTableResponseDTO
        {
            public int Id { get; set; }
            public string TableName { get; set; }
            public List<GetAttributeResponseDTO> Attributes { get; set; }
            public List<ForeignKeyResponseDTO> ForeignKeys { get; set; }
        }
        public record GetAttributeResponseDTO
        {
            public int Id { get; set; }
            public string AttributeName { get; set; }
            public string Type { get; set; }
            public bool PrimaryKey { get; set; }
            public bool NotNull { get; set; }
            public bool Unique { get; set; }
        }
        public record GetAttributeResponseDTO<T> : GetAttributeResponseDTO
        {
            public T DefaultValue { get; set; }
        }
        public record ForeignKeyResponseDTO
        {
            public string ReferencedTableName { get; set; }
            public List<GetReferenceDTO> References { get; set;}
        }

        public record GetReferenceDTO()
        {
            public string From { get; set; }
            public string To { get; set; }
        }
    }
}
