using System.Data;

namespace Metabase.Models.Constraints
{
    public class AttributeCheckConstraint : AttributeConstraint
    {
        public Predicate<SqlDbType> Predicate { get; set; }
    }
}
