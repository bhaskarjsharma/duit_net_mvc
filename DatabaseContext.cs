using duit_net_mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace duit_net_mvc
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Advertisement> Advertisement { get; set; }
    }
}
