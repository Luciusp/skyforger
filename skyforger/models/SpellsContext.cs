using System.Linq;
using Microsoft.EntityFrameworkCore;
using skyforger.models.common.Mana;
using skyforger.models.spells;

namespace skyforger.models
{
    public class SpellsContext : DbContext
    {
        public DbSet<Spell> Spells { get; set; }
        public DbSet<SpellSchool> SpellSchools { get; set; }
        public DbSet<SpellDescriptor> SpellDescriptors { get; set; }
        public DbSet<SpellSubSchool> SpellSubSchools { get; set; }
        public DbSet<ManaType> ManaTypes { get; set; }
        public DbSet<ManaClass> ManaClasses { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=spells.db");
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Cascade;
            }
        }
    }
}