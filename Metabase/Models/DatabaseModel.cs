using Metabase.Models.Constraints;

namespace Metabase.Models
{
    public class DatabaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<RelationModel> Relations { get; set; }
        
    }
}
