using System.ComponentModel.DataAnnotations;
using MossadAgentAPI.Enums;

namespace MossadAgentAPI.Models
{
    public class Agent
    {
        [Key]
        public int Id { get; set; }
        public string Picture { get; set; }
        public string Nickname { get; set; }
        public Location location { get; set; }
        public AgentStatus? Status { get; set; }
    }
}
