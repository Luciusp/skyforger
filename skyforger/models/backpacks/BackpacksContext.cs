using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace skyforger.models.backpacks
{
    public class BackpacksContext : DbContext
    {
        public DbSet<Backpack> Backpacks { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=backpacks.db");
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Cascade;
            }
        }
    }
}