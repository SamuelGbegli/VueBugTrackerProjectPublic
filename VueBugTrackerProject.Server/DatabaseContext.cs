using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VueBugTrackerProject.Classes;

namespace VueBugTrackerProject.Server
{
    /// <summary>
    /// Represents the database within the program
    /// </summary>
    public class DatabaseContext : IdentityDbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Bug> Bugs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { 
        }

    }
}
