using Accessories_Store.Models.Entities;

namespace Accessories_Store.Areas.Admin.Services
{
    public interface IProductMaterialRepo
    {
		IEnumerable<ProductMaterial> findAll();
        Task<ProductMaterial> GetByIdAsync(int? id);
		Task<ProductMaterial> GetByAliasAsync(string? alias);

		Task AddAsync(ProductMaterial productMaterial);
        Task UpdateAsync(ProductMaterial productMaterial);
        Task DeleteAsync(int id);
    }
}
