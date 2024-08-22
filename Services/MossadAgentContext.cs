using Microsoft.EntityFrameworkCore;
using MossadAgentAPI.Models;
using MossadAgentAPI.Enums;

namespace MossadAgentAPI.Services
{
    public class MossadAgentContext : DbContext
    {
        public MossadAgentContext(DbContextOptions<MossadAgentContext> options) : base(options) { 

        try
        {
            if (Database.EnsureCreated())
                {
                if (targets.Count() == 0)
                    {
                    Seedtarget();
                    }
                if (agents.Count() == 0)
                    {
                    Seedagent();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("didnt connect");
            }
        }
        private void Seedtarget()
        {
            Target target = new Target
            {
                Name = "Ismael",
                Role = "Recruit To HamasAlquada Squad",
                Status = Enums.TargetStatus.Die,
            };
        }
        private void Seedagent()
        {
            Agent agent = new Agent
            {
                Nickname = "Islam",
                Picture = "Url.df",
                Status = Enums.AgentStatus.Active,
            };
        }
        public DbSet<Agent> agents { get; set; }
        public DbSet<Mission> missions { get; set; }
        public DbSet<Target> targets { get; set; }

    }
}
