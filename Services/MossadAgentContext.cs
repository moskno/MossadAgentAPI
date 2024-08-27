using Microsoft.EntityFrameworkCore;
using MossadAgentAPI.Models;

namespace MossadAgentAPI.Services
{
    public class MossadAgentContext : DbContext
    {
        public MossadAgentContext(DbContextOptions<MossadAgentContext> options) : base(options)
        {
            try
            {
                Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                Console.WriteLine("didn't connect: " + ex.Message);
            }
        }

        public DbSet<Agent> agents { get; set; }
        public DbSet<Mission> missions { get; set; }
        public DbSet<Target> targets { get; set; }
    }
}
