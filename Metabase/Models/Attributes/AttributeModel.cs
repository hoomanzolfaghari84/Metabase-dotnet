using System.Data;

namespace Metabase.Models.Attributes
{
    public class AttributeModel
    {
        public int Id {  get; set; }
        public string Name { get; set; }
        public SqlDbType Type { get; set; }
        public int? Length { get; set; }
        public RelationModel Relation { get; set; }
        public bool NotNull { get; set; } = false;
        public bool Unique { get; set; } = false;
        public bool PrimaryKey { get; set; } = false;
    }
}
