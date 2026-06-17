using System.ComponentModel.DataAnnotations;

namespace TechMove.Api.Dtos;

public class ContractRequest
{
    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public string Status { get; set; } = string.Empty;

    [Required]
    public string ServiceLevel { get; set; } = string.Empty;

    public string? AgreementFilePath { get; set; }

    [Range(1, int.MaxValue)]
    public int ClientId { get; set; }
}
