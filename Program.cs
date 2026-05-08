using Microsoft.EntityFrameworkCore;
using DevideDemo.Models;
using DevideDemo.Data;
using DevideDemo.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Products.Any())
    {
        db.Products.AddRange(
            new Product { Name = "iPhone 15 Pro", Category = "Smartphones", Price = 89990, Stock = 25, Brand = "Apple" },
            new Product { Name = "Samsung Galaxy S24", Category = "Smartphones", Price = 79990, Stock = 30, Brand = "Samsung" },
            new Product { Name = "Sony WH-1000XM5", Category = "Headphones", Price = 34990, Stock = 50, Brand = "Sony" },
            new Product { Name = "MacBook Pro 14", Category = "Laptops", Price = 199990, Stock = 10, Brand = "Apple" },
            new Product { Name = "Logitech MX Master 3S", Category = "Accessories", Price = 9990, Stock = 100, Brand = "Logitech" }
        );
        db.SaveChanges();
    }
}

app.MapGet("/api/products", async (AppDbContext db, IProductService service) =>
{
    var products = await db.Products.ToListAsync();
    var result = await service.ProcessProductsAsync(products);
    return Results.Ok(result);
});

app.MapGet("/api/products/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    return product is null ? Results.NotFound() : Results.Ok(product);
});

app.MapGet("/api/config", (IConfiguration config) =>
{
    return Results.Ok(new
    {
        AppName = config["AppSettings:AppName"],
        Version = config["AppSettings:Version"]
    });
});

app.Run();