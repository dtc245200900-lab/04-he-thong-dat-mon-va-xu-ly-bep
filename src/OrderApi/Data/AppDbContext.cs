
using Microsoft.EntityFrameworkCore;
using OrderApi.Models;

namespace OrderApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);

            entity.Property(o => o.CustomerName)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            entity.Property(o => o.Status)
                .IsRequired()
                .HasMaxLength(30);

            entity.HasMany(o => o.OrderDetails)
                .WithOne(d => d.Order)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.MenuItemName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(d => d.UnitPrice)
                .HasPrecision(18, 2);

            entity.Property(d => d.LineTotal)
                .HasPrecision(18, 2);
        });
    }
}
