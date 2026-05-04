using System.ComponentModel.DataAnnotations;

namespace AutoszereloMuhely.Dtos;

// Admin által módosítható szerepkör
public class UpdateSzerepDto
{
    [Required]
    public string Szerep { get; set; } = string.Empty; // "Felhasznalo" | "Dolgozo" | "Adminisztrator"
}
