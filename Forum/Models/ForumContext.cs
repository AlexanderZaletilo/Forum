using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Forum.Models
{
    public class ForumContext : IdentityDbContext<User>
    {
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Message> Messages { get; set; } 

        public DbSet<PrivateMessage> PrivateMessages { get; set; }

        public ForumContext(DbContextOptions<ForumContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}