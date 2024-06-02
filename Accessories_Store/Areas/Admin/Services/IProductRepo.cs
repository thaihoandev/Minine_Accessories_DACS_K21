using Accessories_Store.Models.Entities;

namespace Accessories_Store.Areas.Admin.Services
{
    public interface IProductRepo
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetAllByCatAsync(int id);
        Product GetByIdAsync(int? id);
		List<Product> findAll();

		Task AddAsync(Product product);
        Task AddProductVariantAsync(Product product, ProductVariant productVariant);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}
