using Microsoft.EntityFrameworkCore;

class KeyvalueDb : DbContext
{
    public KeyvalueDb(DbContextOptions<KeyvalueDb> options)
        : base(options) { }

    public DbSet<Keyvalue> Keyvalues => Set<Keyvalue>();
}