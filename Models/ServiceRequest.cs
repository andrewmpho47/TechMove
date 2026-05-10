using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechMove.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal CostUSD { get; set; }

        public decimal CostZAR { get; set; }

        [Required]
        public string Status { get; set; }

        // Foreign Key
        public int ContractId { get; set; }

        [ForeignKey("ContractId")]
        public Contract? Contract { get; set; }
    }
}