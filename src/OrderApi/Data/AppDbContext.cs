
using Microsoft.EntityFrameworkCore;
using OrderApi.Models;

namespace OrderApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Bảng lưu thông tin đơn hàng
    public DbSet<Order> Orders => Set<Order>();

    // Bảng lưu chi tiết từng món trong đơn hàng
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Một đơn hàng có nhiều chi tiết món ăn
        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderDetails)
            .WithOne(d => d.Order)
            .HasForeignKey(d => d.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cấu hình độ chính xác tiền
        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderDetail>()
            .Property(d => d.UnitPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderDetail>()
            .Property(d => d.LineTotal)
            .HasPrecision(18, 2);
    }
}
