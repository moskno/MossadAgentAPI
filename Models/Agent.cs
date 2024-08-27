using System.ComponentModel.DataAnnotations;
using MossadAgentAPI.Enums;

namespace MossadAgentAPI.Models
{
    public class Agent
    {
        [Key]
        public int Id { get; set; }
        public string nickname { get; set; }
        public string photoUrl { get; set; }
        public Location? location { get; set; }
        public AgentStatus? Status { get; set; }
    }
}
