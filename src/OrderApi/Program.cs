
using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ==================================================
// 1. DANG KY SQLITE
// ==================================================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

var app = builder.Build();

// ==================================================
// 2. DU LIEU THUC DON 30 MON AN
// ==================================================

var menu = new[]
{
    // MON NUOC
    new MenuItemDto(1, "Pho bo", "Mon nuoc", "Pho bo truyen thong", 45000),
    new MenuItemDto(2, "Pho ga", "Mon nuoc", "Pho ga nuoc dung thanh ngot", 40000),
    new MenuItemDto(3, "Bun bo Hue", "Mon nuoc", "Bun bo Hue cay dam da", 50000),
    new MenuItemDto(4, "Bun rieu cua", "Mon nuoc", "Bun rieu cua dong", 40000),
    new MenuItemDto(5, "Mi Quang", "Mon nuoc", "Mi Quang tom thit", 45000),
    new MenuItemDto(6, "Hu tieu Nam Vang", "Mon nuoc", "Hu tieu day du topping", 45000),

    // COM
    new MenuItemDto(7, "Com tam suon", "Com", "Com tam suon nuong", 40000),
    new MenuItemDto(8, "Com ga xoi mo", "Com", "Com ga chien gion", 50000),
    new MenuItemDto(9, "Com rang dua bo", "Com", "Com rang dua bo", 45000),
    new MenuItemDto(10, "Com chien hai san", "Com", "Com chien tom muc", 55000),
    new MenuItemDto(11, "Com thit kho", "Com", "Com thit kho trung", 40000),
    new MenuItemDto(12, "Com bo luc lac", "Com", "Bo luc lac an kem com", 60000),

    // MON XAO
    new MenuItemDto(13, "Mi xao bo", "Mon xao", "Mi xao bo rau cai", 45000),
    new MenuItemDto(14, "Mi xao hai san", "Mon xao", "Mi xao tom muc", 55000),
    new MenuItemDto(15, "Mien xao cua", "Mon xao", "Mien xao thit cua", 65000),
    new MenuItemDto(16, "Nui xao bo", "Mon xao", "Nui xao bo trung", 40000),
    new MenuItemDto(17, "Rau muong xao toi", "Mon xao", "Rau muong xao toi", 30000),
    new MenuItemDto(18, "Bo xao luc lac", "Mon xao", "Bo xao hanh tay ot chuong", 65000),

    // MON AN THEM
    new MenuItemDto(19, "Cha gio", "Mon an them", "Cha gio chien gion", 35000),
    new MenuItemDto(20, "Goi cuon", "Mon an them", "Goi cuon tom thit", 30000),
    new MenuItemDto(21, "Canh ga chien nuoc mam", "Mon an them", "Canh ga chien gion", 55000),
    new MenuItemDto(22, "Khoai tay chien", "Mon an them", "Khoai tay chien gion", 25000),
    new MenuItemDto(23, "Trung chien", "Mon an them", "Trung ga chien", 20000),
    new MenuItemDto(24, "Dau hu chien", "Mon an them", "Dau hu chien gion", 25000),

    // DO UONG
    new MenuItemDto(25, "Coca Cola", "Do uong", "Nuoc ngot Coca Cola", 15000),
    new MenuItemDto(26, "Tra dao", "Do uong", "Tra dao mat lanh", 25000),
    new MenuItemDto(27, "Tra chanh", "Do uong", "Tra chanh tuoi", 20000),
    new MenuItemDto(28, "Ca phe sua", "Do uong", "Ca phe sua da", 25000),
    new MenuItemDto(29, "Nuoc cam", "Do uong", "Nuoc cam ep tuoi", 30000),
    new MenuItemDto(30, "Nuoc suoi", "Do uong", "Nuoc suoi dong chai", 10000),
    new MenuItemDto(31, "Banh mi thit nuong", "Mon an them",
    "Banh mi kep thit nuong va rau song", 30000)
};

// ==================================================
// 3. KIEM TRA HE THONG
// GET /health
// ==================================================

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "Healthy",
        service = "OrderApi",
        message = "He thong dat mon dang hoat dong"
    });
});

// ==================================================
// 4. LAY DANH SACH MON AN
// GET /menu
// ==================================================

app.MapGet("/menu", () =>
{
    return Results.Ok(menu);
});

// ==================================================
// 5. LAY CHI TIET MON AN
// GET /menu/{id}
// ==================================================

app.MapGet("/menu/{id:int}", (int id) =>
{
    var item = menu.FirstOrDefault(x => x.Id == id);

    if (item == null)
    {
        return Results.NotFound(
            new ApiError("MENU_ITEM_NOT_FOUND", "Khong tim thay mon an.")
        );
    }

    return Results.Ok(item);
});

// ==================================================
// 6. TAO DON HANG
// POST /orders
// ==================================================

app.MapPost("/orders", async (
    OrderRequest request,
    AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.CustomerName))
    {
        return Results.BadRequest(
            new ApiError("INVALID_CUSTOMER",
                "Ten khach hang khong duoc de trong.")
        );
    }

    if (request.Items == null || request.Items.Count == 0)
    {
        return Results.BadRequest(
            new ApiError("EMPTY_ORDER",
                "Don hang phai co it nhat mot mon an.")
        );
    }

    var orderDetails = new List<OrderDetail>();
    decimal totalAmount = 0;

    foreach (var requestedItem in request.Items)
    {
        if (requestedItem.Quantity <= 0)
        {
            return Results.BadRequest(
                new ApiError("INVALID_QUANTITY",
                    "So luong mon an phai lon hon 0.")
            );
        }

        var menuItem = menu.FirstOrDefault(
            x => x.Id == requestedItem.MenuItemId
        );

        if (menuItem == null)
        {
            return Results.NotFound(
                new ApiError("MENU_ITEM_NOT_FOUND",
                    $"Khong tim thay mon an co ID {requestedItem.MenuItemId}.")
            );
        }

        decimal lineTotal = menuItem.Price * requestedItem.Quantity;

        totalAmount += lineTotal;

        orderDetails.Add(new OrderDetail
        {
            MenuItemId = menuItem.Id,
            MenuItemName = menuItem.Name,
            UnitPrice = menuItem.Price,
            Quantity = requestedItem.Quantity,
            LineTotal = lineTotal
        });
    }

    var order = new Order
    {
        CustomerName = request.CustomerName.Trim(),
        TotalAmount = totalAmount,
        Status = "Pending",
        CreatedAt = DateTime.UtcNow,
        OrderDetails = orderDetails
    };

    db.Orders.Add(order);

    await db.SaveChangesAsync();

    var response = ToOrderResponse(order);

    return Results.Created(
        $"/orders/{order.Id}",
        response
    );
});

// ==================================================
// 7. LAY DANH SACH DON HANG
// GET /orders
// ==================================================

app.MapGet("/orders", async (AppDbContext db) =>
{
    var orders = await db.Orders
        .Include(o => o.OrderDetails)
        .OrderByDescending(o => o.Id)
        .ToListAsync();

    var response = orders.Select(ToOrderResponse).ToList();

    return Results.Ok(response);
});

// ==================================================
// 8. LAY CHI TIET DON HANG
// GET /orders/{id}
// ==================================================

app.MapGet("/orders/{id:int}", async (
    int id,
    AppDbContext db) =>
{
    var order = await db.Orders
        .Include(o => o.OrderDetails)
        .FirstOrDefaultAsync(o => o.Id == id);

    if (order == null)
    {
        return Results.NotFound(
            new ApiError("ORDER_NOT_FOUND",
                "Khong tim thay don hang.")
        );
    }

    return Results.Ok(ToOrderResponse(order));
});

// ==================================================
// 9. CAP NHAT TRANG THAI DON HANG
// PUT /orders/{id}/status
// ==================================================

app.MapPut("/orders/{id:int}/status", async (
    int id,
    UpdateOrderStatusRequest request,
    AppDbContext db) =>
{
    var allowedStatuses = new[]
    {
        "Pending",
        "Preparing",
        "Ready",
        "Completed",
        "Cancelled"
    };

    if (string.IsNullOrWhiteSpace(request.Status) ||
        !allowedStatuses.Contains(request.Status,
            StringComparer.OrdinalIgnoreCase))
    {
        return Results.BadRequest(
            new ApiError("INVALID_STATUS",
                "Trang thai khong hop le.")
        );
    }

    var order = await db.Orders
        .Include(o => o.OrderDetails)
        .FirstOrDefaultAsync(o => o.Id == id);

    if (order == null)
    {
        return Results.NotFound(
            new ApiError("ORDER_NOT_FOUND",
                "Khong tim thay don hang.")
        );
    }

    order.Status = allowedStatuses.First(
        s => s.Equals(request.Status,
            StringComparison.OrdinalIgnoreCase)
    );

    await db.SaveChangesAsync();

    return Results.Ok(ToOrderResponse(order));
});

// ==================================================
// 10. CHAY UNG DUNG
// ==================================================

app.Run();

// ==================================================
// 11. CHUYEN ENTITY SANG DTO
// Khong tra truc tiep Entity de tranh JSON CYCLE
// ==================================================

static OrderResponseDto ToOrderResponse(Order order)
{
    return new OrderResponseDto(
        order.Id,
        order.CustomerName,
        order.OrderDetails.Select(d =>
            new OrderDetailResponseDto(
                d.MenuItemId,
                d.MenuItemName,
                d.UnitPrice,
                d.Quantity,
                d.LineTotal
            )
        ).ToList(),
        order.TotalAmount,
        order.Status,
        new DateTimeOffset(
            DateTime.SpecifyKind(order.CreatedAt, DateTimeKind.Utc)
        )
    );
}

// ==================================================
// 12. DTO VA API ERROR
// ==================================================

public record MenuItemDto(
    int Id,
    string Name,
    string Category,
    string Description,
    int Price
);

public record OrderResponseDto(
    int Id,
    string CustomerName,
    List<OrderDetailResponseDto> Items,
    decimal TotalAmount,
    string Status,
    DateTimeOffset CreatedAt
);

public record OrderDetailResponseDto(
    int MenuItemId,
    string MenuItemName,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal
);

public class UpdateOrderStatusRequest
{
    public string Status { get; set; } = "";
}

public record ApiError(
    string Code,
    string Message
);
