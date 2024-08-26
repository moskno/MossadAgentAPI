using MossadAgentAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace MossadAgentAPI.Models
{
    public class Mission
    {
        [Key]
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int TargetId { get; set; }
        public double TimeLeft { get; set; }
        public TimeOnly? ExecutionTime {  get; set; }
        public MissionStatus? Status { get; set; }
    }
}
