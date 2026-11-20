

var builder = WebApplication.CreateBuilder(args);

// Xây dựng ứng dụng
var app = builder.Build();

// ===============================
// DỮ LIỆU THỰC ĐƠN 30 MÓN ĂN
// ===============================

var menu = new[]
{
    // MON NUOC
    new { Id = 1, Name = "Pho bo", Category = "Mon nuoc", Description = "Pho bo truyen thong", Price = 45000 },
    new { Id = 2, Name = "Pho ga", Category = "Mon nuoc", Description = "Pho ga nuoc dung thanh ngot", Price = 40000 },
    new { Id = 3, Name = "Bun bo Hue", Category = "Mon nuoc", Description = "Bun bo Hue cay dam da", Price = 50000 },
    new { Id = 4, Name = "Bun rieu cua", Category = "Mon nuoc", Description = "Bun rieu cua dong", Price = 40000 },
    new { Id = 5, Name = "Mi Quang", Category = "Mon nuoc", Description = "Mi Quang tom thit", Price = 45000 },
    new { Id = 6, Name = "Hu tieu Nam Vang", Category = "Mon nuoc", Description = "Hu tieu day du topping", Price = 45000 },

    // COM
    new { Id = 7, Name = "Com tam suon", Category = "Com", Description = "Com tam suon nuong", Price = 40000 },
    new { Id = 8, Name = "Com ga xoi mo", Category = "Com", Description = "Com ga chien gion", Price = 50000 },
    new { Id = 9, Name = "Com rang dua bo", Category = "Com", Description = "Com rang dua bo", Price = 45000 },
    new { Id = 10, Name = "Com chien hai san", Category = "Com", Description = "Com chien tom muc", Price = 55000 },
    new { Id = 11, Name = "Com thit kho", Category = "Com", Description = "Com thit kho trung", Price = 40000 },
    new { Id = 12, Name = "Com bo luc lac", Category = "Com", Description = "Bo luc lac an kem com", Price = 60000 },

    // MON XAO
    new { Id = 13, Name = "Mi xao bo", Category = "Mon xao", Description = "Mi xao bo rau cai", Price = 45000 },
    new { Id = 14, Name = "Mi xao hai san", Category = "Mon xao", Description = "Mi xao tom muc", Price = 55000 },
    new { Id = 15, Name = "Mien xao cua", Category = "Mon xao", Description = "Mien xao thit cua", Price = 65000 },
    new { Id = 16, Name = "Nui xao bo", Category = "Mon xao", Description = "Nui xao bo trung", Price = 40000 },
    new { Id = 17, Name = "Rau muong xao toi", Category = "Mon xao", Description = "Rau muong xao toi", Price = 30000 },
    new { Id = 18, Name = "Bo xao luc lac", Category = "Mon xao", Description = "Bo xao hanh tay ot chuong", Price = 65000 },

    // MON AN THEM
    new { Id = 19, Name = "Cha gio", Category = "Mon an them", Description = "Cha gio chien gion", Price = 35000 },
    new { Id = 20, Name = "Goi cuon", Category = "Mon an them", Description = "Goi cuon tom thit", Price = 30000 },
    new { Id = 21, Name = "Canh ga chien nuoc mam", Category = "Mon an them", Description = "Canh ga chien gion", Price = 55000 },
    new { Id = 22, Name = "Khoai tay chien", Category = "Mon an them", Description = "Khoai tay chien gion", Price = 25000 },
    new { Id = 23, Name = "Trung chien", Category = "Mon an them", Description = "Trung ga chien", Price = 20000 },
    new { Id = 24, Name = "Dau hu chien", Category = "Mon an them", Description = "Dau hu chien gion", Price = 25000 },

    // DO UONG
    new { Id = 25, Name = "Coca Cola", Category = "Do uong", Description = "Nuoc ngot Coca Cola", Price = 15000 },
    new { Id = 26, Name = "Tra dao", Category = "Do uong", Description = "Tra dao mat lanh", Price = 25000 },
    new { Id = 27, Name = "Tra chanh", Category = "Do uong", Description = "Tra chanh tuoi", Price = 20000 },
    new { Id = 28, Name = "Ca phe sua", Category = "Do uong", Description = "Ca phe sua da", Price = 25000 },
    new { Id = 29, Name = "Nuoc cam", Category = "Do uong", Description = "Nuoc cam ep tuoi", Price = 30000 },
    new { Id = 30, Name = "Nuoc suoi", Category = "Do uong", Description = "Nuoc suoi dong chai", Price = 10000 }
};

// ===============================
// API 1: KIỂM TRA HỆ THỐNG
// ===============================

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "Healthy",
        service = "OrderApi",
        message = "He thong dat mon dang hoat dong"
    });
});

// ===============================
// API 2: LẤY DANH SÁCH MÓN ĂN
// ===============================

app.MapGet("/menu", () =>
{
    return Results.Ok(menu);
});

// ===============================
// API 3: LẤY MÓN ĂN THEO ID
// ===============================

app.MapGet("/menu/{id}", (int id) =>
{
    var item = menu.FirstOrDefault(x => x.Id == id);

    if (item == null)
    {
        return Results.NotFound(new
        {
            message = "Khong tim thay mon an"
        });
    }

    return Results.Ok(item);
});

// ===============================
// API 4: NHẬN YÊU CẦU ĐẶT MÓN
// ===============================

app.MapPost("/orders", (OrderRequest request) =>
{
    if (request.Items == null || request.Items.Count == 0)
    {
        return Results.BadRequest(new
        {
            message = "Don hang phai co it nhat mot mon an"
        });
    }

    return Results.Ok(new
    {
        message = "Dat mon thanh cong",
        customerName = request.CustomerName,
        items = request.Items
    });
});

// ===============================
// CHẠY ỨNG DỤNG
// LUÔN ĐẶT CUỐI CÙNG
// ===============================

app.Run();

// ===============================
// CÁC LỚP DỮ LIỆU
// ===============================

public class OrderRequest
{
    public string CustomerName { get; set; } = "";

    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    public int MenuItemId { get; set; }

    public int Quantity { get; set; }
}
