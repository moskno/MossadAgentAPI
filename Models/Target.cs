using MossadAgentAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace MossadAgentAPI.Models
{
    public class Target
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public Location location { get; set; }
        public TargetStatus Status { get; set; }
    }
}


    