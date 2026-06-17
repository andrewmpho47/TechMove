using System.ComponentModel.DataAnnotations;

namespace TechMove.Api.Dtos;

public class ContractStatusUpdateRequest
{
    [Required]
    public string Status { get; set; } = string.Empty;
}
