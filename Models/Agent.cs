using System.ComponentModel.DataAnnotations;

namespace MossadAgentAPI.Models
{
    public class Agent
    {
        [Key]
        public string Id { get; set; }
        public string Picture { get; set; }
        public string Nickname { get; set; }
        public Location location { get; set; }
        public string Status { get; set; }
    }
}
