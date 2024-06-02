using Accessories_Store.Data_Access;
using Accessories_Store.Models.Entities;
using Microsoft.EntityFrameworkCore;


namespace Accessories_Store.Areas.Admin.Services
{
    public class EFCategoryRepo : ICategoryRepo
    {
        private readonly ApplicationDbContext _context;


        public EFCategoryRepo(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            // return await _context.Categories.ToListAsync();
            return await _context.Categories.Include(x=>x.CategoryType).Where(x=>x.Status == 1).Include(x=>x.Products).ThenInclude(x=>x.ProductVariants).OrderByDescending(x=>x.ParentId).AsNoTracking()
                .ToListAsync();

        }

        public async Task<Category> GetByIdAsync(int? id)
        {
            // return await _context.Products.FindAsync(id);
            // lấy thông tin kèm theo category
            return await _context.Categories.Include(x => x.CategoryType).FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<Category> GetByAliasAsync(string? alias)
        {
            // return await _context.Products.FindAsync(id);
            // lấy thông tin kèm theo category
            return await _context.Categories.Include(x => x.CategoryType).Include(x=>x.Products).FirstOrDefaultAsync(p => p.Alias == alias);
        }

        public async Task AddAsync(Category category)
        {
            category.Status = 1;
            _context.Categories.Add(category);
             await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(Category category)
        {
			category.Status = 1;
			_context.Categories.Update(category);
             await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            category.Status = 1;
            await _context.SaveChangesAsync();
        }

    }
}
