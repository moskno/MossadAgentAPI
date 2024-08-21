using MossadAgentAPI.Enums;

namespace MossadAgentAPI.Models
{
    public class Mission
    {
        public int AgentId { get; set; }
        public TimeOnly TimeLeft { get; set; }
        public MissionStatus Status { get; set; }
    }
}
