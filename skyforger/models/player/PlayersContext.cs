using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace skyforger.models.player
{
    public class PlayersContext: DbContext
    {
        public DbSet<Player> Players { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=players.db");
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Cascade;
            }
        }
    }
}