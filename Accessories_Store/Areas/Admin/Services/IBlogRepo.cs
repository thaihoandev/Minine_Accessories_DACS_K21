using Accessories_Store.Models.Entities;

namespace Accessories_Store.Areas.Admin.Services
{
    public interface IBlogRepo
    {

        Task<IEnumerable<Blog>> GetAllAsync();
        Task<Blog> GetByIdAsync(int? id);
        Blog GetById(int? id);
        Task AddAsync(Blog blog);
        Task UpdateAsync(Blog blog);
        Task DeleteAsync(int id);
    }
}
