namespace Metabase.Models.Constraints
{
    public abstract class RelationConstraint : ConstraintModel
    {
        public RelationModel Relation { get; set; }
    }
}
