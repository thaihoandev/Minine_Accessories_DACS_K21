using Accessories_Store.Data_Access;
using Accessories_Store.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Accessories_Store.Areas.Admin.Services
{
    public class EFCommentRepo : ICommentRepo
    {
        private readonly ApplicationDbContext _context;


        public EFCommentRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            // return await _context.Products.ToListAsync();
            return await _context.Comments.Include(x=>x.ApplicationUser).ToListAsync();
        }
    }
}
