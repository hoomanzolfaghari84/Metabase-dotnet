using Metabase.Models.Attributes;

namespace Metabase.Models.Constraints
{
    public abstract class AttributeConstraint : ConstraintModel
    {
        public AttributeModel Attribute { get; set; }

    }


}
