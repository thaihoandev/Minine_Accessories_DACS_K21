using Accessories_Store.Data_Access;
using Accessories_Store.Helpers;
using Accessories_Store.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace Accessories_Store.Areas.Admin.Services
{
    public class EFProductRepo : IProductRepo
    {
        private readonly ApplicationDbContext _context;


        public EFProductRepo(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            // return await _context.Products.ToListAsync();
            return await _context.Products.Include(p => p.Category ).Include(p => p.ProductVariants).Where(x=>x.Status == 1).ToListAsync();

        }


		public async Task<IEnumerable<Product>> GetAllByCatAsync(int id)
        {
            // return await _context.Products.ToListAsync();
            return await _context.Products.Where(x => x.CategoryId == id && x.Status == 1)
                .Include(x => x.Category)
                .OrderByDescending(x => x.Id).ToListAsync();

        }

        public Product GetByIdAsync(int? id)
        {
			return _context.Products
				   .Include(p => p.Category)
				   .Include(p => p.ProductImages)
				   .Include(p => p.ProductVariants)
				   .Include(p => p.ProductMaterial)
				   .FirstOrDefault(x => x.Id == id);
		}

        public List<Product> findAll ()
		{
			var products = _context.Products.Include(p => p.ProductVariants).Include(p => p.Category).Include(x=>x.Ratings).Where(x => x.Published == true && x.Status == 1 && x.ProductVariants.Count() >0).ToList();
			// Include ProductVariants

			return products;
		}


		public async Task AddAsync(Product product)
        {
            product.Status = Status.StatusOk;
            
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
        public async Task AddProductVariantAsync(Product product, ProductVariant productVariant)
        {
            productVariant.Status = Status.StatusOk;
            product.ProductVariants.Add(productVariant);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            product.Status = Status.StatusOk;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.Status = Status.StatusCancel;
            }
            await _context.SaveChangesAsync();
        }

    }
}
