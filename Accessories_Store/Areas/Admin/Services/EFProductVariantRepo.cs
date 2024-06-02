using Accessories_Store.Data_Access;
using Accessories_Store.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Accessories_Store.Areas.Admin.Services
{
    public class EFProductVariantRepo : IProductVariantRepo
    {
        private readonly ApplicationDbContext _context;


        public EFProductVariantRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductVariant>> GetAllAsync()
        {
            // return await _context.ProductVariants.ToListAsync();
            return await _context.ProductVariants.ToListAsync();

        }
        public async Task<IEnumerable<ProductVariant>> GetAllByProductIdAsync(int id)
        {
            // return await _context.ProductVariants.ToListAsync();
            return await _context.ProductVariants.Include(p => p.Product).Where(x=>x.ProductId == id).ToListAsync();

        }

        public ProductVariant GetById(int? id)
        {
			return _context.ProductVariants.Include(p => p.Product).ThenInclude(x => x.Category)
                                            .Include(p => p.Product).ThenInclude(x => x.ProductMaterial)
											.Include(p => p.Product).ThenInclude(x => x.ProductImages)
											.Where(x => x.Id == id).FirstOrDefault();
		}

		public dynamic GetProductDetailsById(int? id)
		{
			return _context.ProductVariants.Where(x=>x.ProductId == id).Select(a => new
			{
				Id = a.Id,
				CreatedAt = a.CreatedAt,
				Price = a.Price,
				Quantity = a.Quantity,
                Status = a.Status,
				ProductSize = a.ProductSize,
				Product = a.Product,
                CategoryName = a.Product.Category.Name,
			}).ToList();
		}

		public async Task AddAsync(ProductVariant productVariant)
        {
            productVariant.Status = 1;
            _context.ProductVariants.Add(productVariant);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(ProductVariant productVariant)
        {
            productVariant.Status = 1;
            _context.ProductVariants.Update(productVariant);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var productVariant = await _context.ProductVariants.FindAsync(id);
            if (productVariant != null)
            {
                productVariant.Status = 0;
            }
            await _context.SaveChangesAsync();
        }

    }
}
