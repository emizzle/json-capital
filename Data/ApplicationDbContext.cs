using JSONCapital.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JSONCapital.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:JSONCapital.Data.ApplicationDbContext"/> class.
        /// </summary>
        /// <param name="options">Options.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            if(!options.IsFrozen) Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            // add index (unique value) constraint to CoinTrackingTradeID
            builder.Entity<Trade>().HasIndex(t => t.CoinTrackingTradeID).IsUnique();
        }

        public virtual DbSet<Trade> Trades { get; set; }
    }
}
