using MossadAgentAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace MossadAgentAPI.Models
{
    public class Target
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }
        public string position { get; set; }
        public string photoUrl { get; set; }
        public Location? location { get; set; }
        public TargetStatus? Status { get; set; }
    }
}


    