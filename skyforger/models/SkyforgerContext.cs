using Microsoft.EntityFrameworkCore;
using skyforger.models.spells;

namespace skyforger.models
{
    public class SkyforgerContext : DbContext
    {
        public DbSet<Spell> Spells { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=source.db");
    }
}