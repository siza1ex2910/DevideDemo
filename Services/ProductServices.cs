using DevideDemo.Models;

namespace DevideDemo.Services;

public class ProductService : IProductService
{
    public Task<List<Product>> ProcessProductsAsync(List<Product> products)
    {
        var sorted = products.OrderByDescending(p => p.Price).ToList();
        return Task.FromResult(sorted);
    }
}