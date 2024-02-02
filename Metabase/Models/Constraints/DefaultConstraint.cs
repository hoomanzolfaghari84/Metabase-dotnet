using System.Data;
using System.Data.SqlTypes;

namespace Metabase.Models.Constraints
{
    public class DefaultConstraint : AttributeConstraint
    {
        
        public byte[] Value { get; set; }
        public SqlDbType Type { get; set; }
        
    }
}
