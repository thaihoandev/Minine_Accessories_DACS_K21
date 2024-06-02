using Accessories_Store.Data_Access;
using Accessories_Store.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Accessories_Store.Areas.Admin.Controllers
{
	public class AdminSearchController : Controller
	{
		private readonly ApplicationDbContext _context;

		public AdminSearchController(ApplicationDbContext context)
		{
			_context = context;
		}
		//Get: Search/FindProduct
		[HttpPost]
		[Route("/admin/products/find-result")]
		public IActionResult FindResult(string keyword)
		{
			List<Product> lsAll = new List<Product>();
			List<Product> ls = new List<Product>();

			lsAll = _context.Products.AsNoTracking().Include(x => x.Category).Include(x=>x.ProductVariants).ToList();

			if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
			{
				return PartialView("~/Areas/Admin/Views/AdminSearch/_ListProductSearchPartialView.cshtml", lsAll);
			}
			ls = _context.Products.AsNoTracking().
				Include(x => x.Category).Include(x => x.ProductVariants)
                .Where(x => x.Name.Contains(keyword))
				.OrderByDescending(x => x.Name)
				.Take(10)
				.ToList();

			if (ls == null)
			{
				return PartialView("~/Areas/Admin/Views/AdminSearch/_ListProductSearchPartialView.cshtml", null);

			}
			else
			{
				return PartialView("~/Areas/Admin/Views/AdminSearch/_ListProductSearchPartialView.cshtml", ls);

			}

		}

		[HttpPost]
		[Route("/admin/orders/find-result")]
		public IActionResult FindResultOrder(string keyword)
		{
			List<Order> lsAll = new List<Order>();
			List<Order> ls = new List<Order>();

			lsAll = _context.Orders.AsNoTracking().Include(x => x.ApplicationUser).ToList();

			if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
			{
				return PartialView("~/Areas/Admin/Views/AdminSearch/_ListOrderSearchPartialView.cshtml", lsAll);
			}
			ls = _context.Orders.AsNoTracking().
				Include(x => x.ApplicationUser)
				.Where(x => x.Id.ToString().Contains(keyword))
				.OrderByDescending(x => x.Name)
				.Take(10)
				.ToList();

			if (ls == null)
			{
				return PartialView("~/Areas/Admin/Views/AdminSearch/_ListOrderSearchPartialView.cshtml", null);

			}
			else
			{
				return PartialView("~/Areas/Admin/Views/AdminSearch/_ListOrderSearchPartialView.cshtml", ls);

			}

		}
	}
}
