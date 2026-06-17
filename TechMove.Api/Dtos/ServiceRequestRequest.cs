using System.ComponentModel.DataAnnotations;

namespace TechMove.Api.Dtos;

public class ServiceRequestRequest
{
    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal CostUSD { get; set; }

    [Required]
    public string Status { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int ContractId { get; set; }
}
