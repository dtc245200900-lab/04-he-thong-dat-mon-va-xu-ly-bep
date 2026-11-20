
namespace OrderApi.Models;

public class Order
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = "";

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = "Cho tiep nhan";

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<OrderDetail> OrderDetails { get; set; } = new();
}
