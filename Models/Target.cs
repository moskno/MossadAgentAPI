using System.ComponentModel.DataAnnotations;

namespace MossadAgentAPI.Models
{
    public class Target
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public Location location { get; set; }
        public string Status { get; set; }
    }
}
    