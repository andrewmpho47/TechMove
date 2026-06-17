using System.ComponentModel.DataAnnotations;

namespace TechMove.Api.Dtos;

public class ClientRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string ContactDetails { get; set; } = string.Empty;

    [Required]
    public string Region { get; set; } = string.Empty;
}
