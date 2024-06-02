using Accessories_Store.Data_Access;
using Accessories_Store.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Accessories_Store.Areas.Admin.Services
{
    public class EFRatingRepo : IRatingRepo
    {
        private readonly ApplicationDbContext _context;


        public EFRatingRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public dynamic findAllById(int id)
        {
			return _context.Ratings.Include(x=>x.Product).Include(x=>x.ApplicationUser).Where(x=>x.Status ==1 && x.ProductId==id).OrderByDescending(x=>x.CreatedAt).Select(a => new
			{
				Id = a.Id,
				Content = a.Content,
				NumberStar = a.NumberStars,
				Status = a.Status,
				UserId = a.ApplicationUser.Id,
				ApplicationUser = a.ApplicationUser,
				CreatedAt = a.CreatedAt,
			}).ToList();


		}


	}
}
