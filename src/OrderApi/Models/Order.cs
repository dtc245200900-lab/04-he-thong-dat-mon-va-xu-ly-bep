
namespace OrderApi.Models;

public class Order
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = "";

    public decimal TotalAmount { get; set; }

    // Pending, Preparing, Ready, Completed, Cancelled
    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<OrderDetail> OrderDetails { get; set; } = new();
}
