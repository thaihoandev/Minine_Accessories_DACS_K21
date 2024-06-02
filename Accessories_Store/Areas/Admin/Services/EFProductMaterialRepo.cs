using Accessories_Store.Data_Access;
using Accessories_Store.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Accessories_Store.Areas.Admin.Services
{
    public class EFProductMaterialRepo : IProductMaterialRepo
	{
        private readonly ApplicationDbContext _context;


        public EFProductMaterialRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ProductMaterial> findAll()
        {
			return _context.ProductMaterials.Where(x=>x.Status ==1).OrderByDescending(x=>x.CreatedAt).ToList();
		}


        public async Task<ProductMaterial> GetByIdAsync(int? id)
        {
            // return await _context.Products.FindAsync(id);
            // lấy thông tin kèm theo ProductMaterial
            return await _context.ProductMaterials.Include(x=>x.Products).FirstOrDefaultAsync(p => p.Id == id);
        }
		public async Task<ProductMaterial> GetByAliasAsync(string? alias)
		{
			// return await _context.Products.FindAsync(id);
			// lấy thông tin kèm theo ProductMaterial
			return await _context.ProductMaterials.Include(x => x.Products).ThenInclude(x=>x.ProductVariants).AsNoTracking().FirstOrDefaultAsync(p => p.Alias == alias);
		}
		public async Task AddAsync(ProductMaterial productMaterial)
        {
            productMaterial.Status = 1;
            _context.ProductMaterials.Add(productMaterial);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(ProductMaterial productMaterial)
        {
            productMaterial.Status = 1;
            _context.ProductMaterials.Update(productMaterial);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var productMaterial = await _context.ProductMaterials.FindAsync(id);
            productMaterial.Status = -1;
            await _context.SaveChangesAsync();
        }


    }
}
