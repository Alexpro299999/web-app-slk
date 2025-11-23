using Microsoft.EntityFrameworkCore;
using MyWebApp.Models;

namespace MyWebApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tariff> Tariffs { get; set; }
    public DbSet<Equipment> Equipments { get; set; }
    public DbSet<Subscriber> Subscribers { get; set; }
}