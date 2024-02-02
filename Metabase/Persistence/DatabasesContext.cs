using Microsoft.EntityFrameworkCore;

namespace Metabase.Persistence
{
    public class DatabasesContext : DbContext
    {
        private readonly string _connectionString;

        public DatabasesContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
