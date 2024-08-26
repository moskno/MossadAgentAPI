using MossadAgentAPI.Enums;

namespace MossadAgentAPI.Models
{
    public class MissionDetails
        {
        public int MissionId { get; set; }
        public double TimeLeft { get; set; }
        public TimeOnly ExecutionTime { get; set; }
        public MissionStatus MissionStatus { get; set; }

        public string AgentNickname { get; set; }
        public Location AgentLocation { get; set; }
        public AgentStatus AgentStatus { get; set; }

        public string TargetName { get; set; }
        public string TargetRole { get; set; }
        public Location TargetLocation { get; set; }
        public TargetStatus TargetStatus { get; set; }
        public double Distance { get; set; }
    }
}
