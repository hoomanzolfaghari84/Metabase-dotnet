using Metabase.Models.Attributes;

namespace Metabase.Models.Constraints
{
    public class ForeignKeyConstraint : RelationConstraint
    {
        public RelationModel ReferencedRelation { get; set; }
        public IEnumerable<FKReference> References { get; set; }
        //public IEnumerable<AttributeModel> ReferencedAttributes {  get; set; }
        //public IEnumerable<AttributeModel> ReferencingAttributes { get; set; }

    }

    public class FKReference
    {
        public int ForeignKeyConstraintId { get; set; }
        public ForeignKeyConstraint ForeignKeyConstraint { get; set; }
        public int ReferencingAttributeId { get; set; }
        public AttributeModel ReferencingAttribute { get; set; }
        public int ReferencedAttributeId { get; set; }
        public AttributeModel ReferencedAttribute { get; set; }
    }
}
