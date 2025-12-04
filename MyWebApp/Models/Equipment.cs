using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;

namespace MyWebApp.Models;

public class Equipment
{
    public int Id { get; set; }

    [Required]
    public string ModelName { get; set; } = string.Empty;

    [Required]
    public string SerialNumber { get; set; } = string.Empty;

    public string Type { get; set; } = "Router";

    public bool IsInStock { get; set; } = true;

    [Column(TypeName = "vector(3)")]
    public Vector? Embedding { get; set; }
}