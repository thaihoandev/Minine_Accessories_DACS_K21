using Accessories_Store.Data_Access;
using Accessories_Store.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Accessories_Store.Areas.Admin.Services
{
    public class EFBlogRepo : IBlogRepo
    {
        private readonly ApplicationDbContext _context;


        public EFBlogRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Blog>> GetAllAsync()
        {
            // return await _context.Products.FindAsync(id);
            // lấy thông tin kèm theo ProductMaterial
            return await _context.Blogs.Include(x => x.ApplicationUser).Include(x=>x.Comments).ToListAsync();
        }
     
		public async Task<Blog> GetByIdAsync(int? id)
		{
			// return await _context.Products.FindAsync(id);
			// lấy thông tin kèm theo ProductMaterial
			return await _context.Blogs.Include(x => x.ApplicationUser).FirstOrDefaultAsync(p => p.Id == id);
		}

        public Blog GetById(int? id)
        {
            // return await _context.Products.FindAsync(id);
            // lấy thông tin kèm theo ProductMaterial
            return _context.Blogs.Include(x => x.ApplicationUser).FirstOrDefault(p => p.Id == id);
        }
        public async Task AddAsync(Blog blog)
		{
			blog.Status = 1;
			_context.Blogs.Add(blog);
			await _context.SaveChangesAsync();
		}


		public async Task UpdateAsync(Blog blog)
		{
			blog.Status = 1;
			_context.Blogs.Update(blog);
			await _context.SaveChangesAsync();
		}
		public async Task DeleteAsync(int id)
		{
			var blog = await _context.Blogs.FindAsync(id);
			blog.Status = 1;
			await _context.SaveChangesAsync();
		}
	}
}
