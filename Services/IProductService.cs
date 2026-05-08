using DevideDemo.Models;

namespace DevideDemo.Services;

public interface IProductService
{
    Task<List<Product>> ProcessProductsAsync(List<Product> products);
}