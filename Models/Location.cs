using System.ComponentModel.DataAnnotations;

namespace MossadAgentAPI.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [Range(0, 1000, ErrorMessage = "X must be between 0 and 1000.")]
        public int x { get; set; }

        [Range(0, 1000, ErrorMessage = "Y must be between 0 and 1000.")]
        public int y { get; set; }
    }
}
