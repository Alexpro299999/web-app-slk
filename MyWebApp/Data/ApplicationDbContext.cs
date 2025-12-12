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

    public DbSet<MetaTable> MetaTables { get; set; }
    public DbSet<MetaColumn> MetaColumns { get; set; }
    public DbSet<MetaRow> MetaRows { get; set; }
    public DbSet<MetaValue> MetaValues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<MetaValue>()
            .HasOne(v => v.Row)
            .WithMany(r => r.Values)
            .HasForeignKey(v => v.MetaRowId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MetaColumn>()
            .HasOne(c => c.Table)
            .WithMany(t => t.Columns)
            .HasForeignKey(c => c.MetaTableId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MetaRow>()
            .HasOne(r => r.Table)
            .WithMany()
            .HasForeignKey(r => r.MetaTableId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}