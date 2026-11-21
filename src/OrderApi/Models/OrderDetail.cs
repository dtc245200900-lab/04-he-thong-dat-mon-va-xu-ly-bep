
namespace OrderApi.Models;

public class OrderDetail
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int MenuItemId { get; set; }

    public string MenuItemName { get; set; } = "";

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal LineTotal { get; set; }

    // Quan hệ với đơn hàng
    public Order? Order { get; set; }
}
