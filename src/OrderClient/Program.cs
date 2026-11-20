
using System.Net.Http.Json;

// Địa chỉ của Order API
string baseUrl = args.Length > 0
    ? args[0]
    : "http://localhost:5258";

// Tạo HTTP Client
using var client = new HttpClient();

try
{
    // Gửi GET Request đến API để lấy danh sách món ăn
    var menu = await client.GetFromJsonAsync<MenuItem[]>(
        $"{baseUrl}/menu"
    );

    Console.WriteLine("===== DANH SACH MON AN =====");

    if (menu != null)
    {
        foreach (var item in menu)
        {
            Console.WriteLine(
                $"{item.Id}. {item.Name} - {item.Price:N0} VND"
            );

            Console.WriteLine($"   Danh muc: {item.Category}");
            Console.WriteLine($"   Mo ta: {item.Description}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine("Khong the ket noi den Order API!");
    Console.WriteLine(ex.Message);
}

// Lớp dữ liệu nhận từ API
public class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public int Price { get; set; }
}
