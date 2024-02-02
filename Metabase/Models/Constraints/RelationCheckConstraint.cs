using System.Data;

namespace Metabase.Models.Constraints
{
    public class RelationCheckConstraint : RelationConstraint
    {
        public Predicate<SqlDbType> Predicate { get; set; }
    }
}
