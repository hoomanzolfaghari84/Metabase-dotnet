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
}
