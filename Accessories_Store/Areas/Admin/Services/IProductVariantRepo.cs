using Accessories_Store.Models.Entities;

namespace Accessories_Store.Areas.Admin.Services
{
    public interface IProductVariantRepo
    {
        Task<IEnumerable<ProductVariant>> GetAllAsync();
        Task<IEnumerable<ProductVariant>> GetAllByProductIdAsync(int id);
        dynamic GetProductDetailsById(int? id);
		ProductVariant GetById(int? id);
        Task AddAsync(ProductVariant productVariant);
        Task UpdateAsync(ProductVariant productVariant);
        Task DeleteAsync(int id);
    }
}
