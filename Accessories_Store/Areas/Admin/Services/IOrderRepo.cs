using Accessories_Store.Models.Entities;

namespace Accessories_Store.Areas.Admin.Services
{
    public interface IOrderRepo
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetAllByStatusAsync(int id);
        Task<IEnumerable<OrderDetail>> GetAllDetailsByAsync(int? id);
        Task<Order> GetByIdAsync(int? id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(int id);
    }
}
