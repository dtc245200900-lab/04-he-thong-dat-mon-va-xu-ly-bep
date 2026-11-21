
using System.Net.Http.Json;
using System.Text.Json;

// Địa chỉ Order API
string baseUrl = args.Length > 0
    ? args[0]
    : "http://localhost:5258";

using var client = new HttpClient();

try
{
    // ==================================================
    // 1. LẤY DANH SÁCH MÓN ĂN
    // GET /menu
    // ==================================================

    var menu = await client.GetFromJsonAsync<MenuItem[]>(
        $"{baseUrl}/menu"
    );

    Console.WriteLine("===== DANH SACH MON AN =====");

    if (menu == null || menu.Length == 0)
    {
        Console.WriteLine("Khong co mon an nao!");
        return;
    }

    foreach (var item in menu)
    {
        Console.WriteLine(
            $"{item.Id}. {item.Name} - {item.Price:N0} VND"
        );
    }

    // ==================================================
    // 2. NHẬP THÔNG TIN KHÁCH HÀNG
    // ==================================================

    Console.WriteLine("\n===== DAT MON =====");

    Console.Write("Nhap ten khach hang: ");
    string customerName = Console.ReadLine() ?? "";

    Console.Write("Nhap ID mon an: ");
    int menuItemId = int.TryParse(
        Console.ReadLine(), out int id
    ) ? id : 0;

    // ==================================================
    // 3. LẤY CHI TIẾT MÓN ĂN
    // GET /menu/{id}
    // ==================================================

    Console.WriteLine("\n===== CHI TIET MON AN =====");

    var detailResponse = await client.GetAsync(
        $"{baseUrl}/menu/{menuItemId}"
    );

    Console.WriteLine(
        $"HTTP Status: {(int)detailResponse.StatusCode}"
    );

    string detailContent =
        await detailResponse.Content.ReadAsStringAsync();

    // Kiểm tra lỗi khi lấy chi tiết món ăn
    if (!detailResponse.IsSuccessStatusCode)
    {
        Console.WriteLine("Khong tim thay mon an!");
        Console.WriteLine("Thong tin loi:");

        try
        {
            using var errorDocument =
                JsonDocument.Parse(detailContent);

            var errorRoot = errorDocument.RootElement;

            if (errorRoot.TryGetProperty("code", out var code))
                Console.WriteLine($"Ma loi: {code}");

            if (errorRoot.TryGetProperty("message", out var message))
                Console.WriteLine($"Thong bao: {message}");
        }
        catch
        {
            Console.WriteLine(detailContent);
        }

        return;
    }

    // Chuyển JSON thành đối tượng MenuItem
    var selectedItem =
        JsonSerializer.Deserialize<MenuItem>(
            detailContent,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        );

    if (selectedItem == null)
    {
        Console.WriteLine("Khong doc duoc thong tin mon an!");
        return;
    }

    // Hiển thị thông tin chi tiết món ăn
    Console.WriteLine($"Ma mon: {selectedItem.Id}");
    Console.WriteLine($"Ten mon: {selectedItem.Name}");
    Console.WriteLine($"Danh muc: {selectedItem.Category}");
    Console.WriteLine($"Mo ta: {selectedItem.Description}");
    Console.WriteLine($"Don gia: {selectedItem.Price:N0} VND");

    // ==================================================
    // 4. NHẬP SỐ LƯỢNG
    // ==================================================

    Console.Write("\nNhap so luong: ");
    int quantity = int.TryParse(
        Console.ReadLine(), out int qty
    ) ? qty : 0;

    // ==================================================
    // 5. TẠO DỮ LIỆU ĐƠN HÀNG
    // ==================================================

    var orderRequest = new
    {
        customerName = customerName,
        items = new[]
        {
            new
            {
                menuItemId = menuItemId,
                quantity = quantity
            }
        }
    };

    // ==================================================
    // 6. GỬI POST ĐẾN ORDER API
    // POST /orders
    // ==================================================

    var response = await client.PostAsJsonAsync(
        $"{baseUrl}/orders",
        orderRequest
    );

    Console.WriteLine("\n===== KET QUA DAT MON =====");
    Console.WriteLine($"HTTP Status: {(int)response.StatusCode}");

    string responseContent =
        await response.Content.ReadAsStringAsync();

    // ==================================================
    // 7. XỬ LÝ KẾT QUẢ ĐẶT MÓN
    // ==================================================

    if (response.IsSuccessStatusCode)
    {
        using var document = JsonDocument.Parse(responseContent);
        var root = document.RootElement;

        Console.WriteLine("Dat mon thanh cong!");

        if (root.TryGetProperty("id", out var orderId))
            Console.WriteLine($"Ma don hang: {orderId}");

        if (root.TryGetProperty("customerName", out var name))
            Console.WriteLine($"Khach hang: {name}");

        if (root.TryGetProperty("totalAmount", out var total))
            Console.WriteLine(
                $"Tong tien: {total.GetDecimal():N0} VND"
            );

        if (root.TryGetProperty("status", out var status))
            Console.WriteLine($"Trang thai: {status}");
    }
    else
    {
        Console.WriteLine("Dat mon that bai!");
        Console.WriteLine("Thong tin loi:");

        try
        {
            using var errorDocument =
                JsonDocument.Parse(responseContent);

            var errorRoot = errorDocument.RootElement;

            if (errorRoot.TryGetProperty("code", out var code))
                Console.WriteLine($"Ma loi: {code}");

            if (errorRoot.TryGetProperty("message", out var message))
                Console.WriteLine($"Thong bao: {message}");
        }
        catch
        {
            Console.WriteLine(responseContent);
        }
    }
}
catch (HttpRequestException ex)
{
    Console.WriteLine("Khong the ket noi den Order API!");
    Console.WriteLine(ex.Message);
}
catch (Exception ex)
{
    Console.WriteLine("Da xay ra loi:");
    Console.WriteLine(ex.Message);
}

// ==================================================
// 8. MODEL NHẬN DANH SÁCH VÀ CHI TIẾT MÓN ĂN
// ==================================================

public class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public int Price { get; set; }
}
