using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models;

public class Subscriber
{
    public int Id { get; set; }

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    public string ContractNumber { get; set; } = string.Empty;

    public int TariffId { get; set; }

    [ForeignKey("TariffId")]
    public Tariff? Tariff { get; set; }
}