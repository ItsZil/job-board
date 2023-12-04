using job_board.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace job_board.Utilities
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Education> Education { get; set; }
        public DbSet<JobHistory> JobHistory { get; set; }
        
        
        public DbSet<Company> Companies { get; set; }
        
        public DbSet<Ad> Ads { get; set; }
        public DbSet<Application> Applications { get; set; }

    }
}
