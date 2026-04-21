using System.ComponentModel.DataAnnotations;

namespace AutoszereloMuhely.Dtos;

public class CreateUgyfelDto
{
    [Required]
    [RegularExpression(@".*\S.*")]
    public string Nev { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@".*\S.*")]
    public string Lakcim { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}