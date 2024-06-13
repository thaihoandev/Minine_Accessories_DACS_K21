    using Accessories_Store.Data_Access;
using Accessories_Store.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Accessories_Store.Areas.Admin.Services
{
    public class EFOrderRepo : IOrderRepo
    {
        private readonly ApplicationDbContext _context;


        public EFOrderRepo(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            // return await _context.Products.ToListAsync();
            return await _context.Orders.Include(x=>x.OrderDetails).ThenInclude(x=>x.Product).ThenInclude(x=>x.Category).OrderByDescending(x => x.Status).OrderByDescending(x=>x.CreatedAt).ToListAsync();

        }
        public async Task<IEnumerable<Order>> GetAllByStatusAsync(int id)
        {
            // return await _context.Products.ToListAsync();
            return await _context.Orders
                .OrderByDescending(x => x.Id).ToListAsync();

        }
        public async Task<IEnumerable<OrderDetail>> GetAllDetailsByAsync(int? id)
        {
            // return await _context.Products.ToListAsync();
            return await _context.OrderDetails.Where(x => x.OrderId == id)
                .OrderByDescending(x => x.Id).ToListAsync();

        }
        public async Task<Order> GetByIdAsync(int? id)
        {
            // return await _context.Orders.FindAsync(id);
            // lấy thông tin kèm theo Orders
            return await _context.Orders.Include(x=>x.ApplicationUser).Include(x=>x.OrderDetails).ThenInclude(x=>x.Product).ThenInclude(x=>x.ProductMaterial).FirstOrDefaultAsync(p => p.Id == id);
        }


        public async Task AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }

    }
}
