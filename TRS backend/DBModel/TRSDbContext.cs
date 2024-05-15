using Microsoft.EntityFrameworkCore;

namespace TRS_backend.DBModel
{
    public class TRSDbContext : DbContext
    {

        public TRSDbContext() { }

        public TRSDbContext(DbContextOptions<TRSDbContext> options) : base(options) { }

        public DbSet<TblUser> Users { get; set; }
    }
}
