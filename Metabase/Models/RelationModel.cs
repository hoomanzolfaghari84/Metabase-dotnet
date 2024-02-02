using Metabase.Models.Attributes;
using Metabase.Models.Constraints;

namespace Metabase.Models
{
    public class RelationModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DatabaseModel Database { get; set; }
        public IEnumerable<AttributeModel> Attributes { get; set; } = new List<AttributeModel>();
        //public IEnumerable<RelationConstraint> RelationConstraints { get; set; }

    }
}
