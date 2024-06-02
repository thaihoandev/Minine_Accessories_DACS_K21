using Accessories_Store.Models.Entities;

namespace Accessories_Store.Areas.Admin.Services
{
    public interface ICategoryRepo
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(int? id);
        Task<Category> GetByAliasAsync(string? alias);

        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
    }
}
