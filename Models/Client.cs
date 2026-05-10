using System.ComponentModel.DataAnnotations;

namespace TechMove.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ContactDetails { get; set; }

        [Required]
        public string Region { get; set; }

        public List<Contract>? Contracts { get; set; }
    }
}