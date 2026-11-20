
namespace OrderApi.Models;

public class OrderRequest
{
    public string CustomerName { get; set; } = "";

    public List<OrderItemRequest> Items { get; set; } = new();
}
