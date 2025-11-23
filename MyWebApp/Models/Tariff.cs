using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models;

public class Tariff
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(1, 10000)]
    public int SpeedMbps { get; set; }

    [DataType(DataType.Currency)]
    public decimal Price { get; set; }
}