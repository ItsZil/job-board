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
    }
}
