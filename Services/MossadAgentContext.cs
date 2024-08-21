using Microsoft.EntityFrameworkCore;
using MossadAgentAPI.Models;

namespace MossadAgentAPI.Services
{
    public class MossadAgentContext : DbContext
    {
        public MossadAgentContext(DbContextOptions<MossadAgentContext> options) : base(options) { }

        public DbSet<Agent> agents { get; set; }
        public DbSet<Mission> missions { get; set; }
        public DbSet<Target> targets { get; set; }

    }
}
