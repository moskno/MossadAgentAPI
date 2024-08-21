using MossadAgentAPI.Enums;

namespace MossadAgentAPI.Models
{
    public class Mission
    {
        public int AgentId { get; set; }
        public int TargetId { get; set; }
        public TimeOnly TimeLeft { get; set; }
        public TimeOnly ExecutionTime {  get; set; }
        public MissionStatus Status { get; set; }
    }
}
