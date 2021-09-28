using Microsoft.EntityFrameworkCore;
using Models.UserMicroservice;

namespace UserMicroservice.Configuration
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options):base(options)
        {

        }

        public DbSet<Interest> Interests { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<InterestByUser> InterestsByUsers { get; set; }
    }
}
