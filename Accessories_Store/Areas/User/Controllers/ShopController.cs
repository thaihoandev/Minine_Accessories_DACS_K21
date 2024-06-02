using Accessories_Store.Areas.Admin.Services;
using Accessories_Store.Data_Access;
using Accessories_Store.Helpers;
using Accessories_Store.Models.Entities;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace Accessories_Store.Areas.User.Controllers
{
	[Area("User")]
    public class ShopController : Controller
    {
		private readonly IProductRepo _productContext;
		private readonly IProductVariantRepo _productVariantContext;
		private readonly IProductMaterialRepo _productMaterialContext;

		private readonly ICategoryRepo _categoryContext;
		private readonly ApplicationDbContext _context;

		public INotyfService _notifyService { get; }
		public ShopController(ApplicationDbContext context, INotyfService notyfService, IProductRepo productContext, ICategoryRepo categoryContext, IProductVariantRepo productVariantContext, IProductMaterialRepo productMaterialContext)
		{
			_context = context;
			_notifyService = notyfService;
			_productContext = productContext;
			_categoryContext = categoryContext;
			_productVariantContext = productVariantContext;
			_productMaterialContext = productMaterialContext;
		}

		[Route("accessories")]
		public IActionResult Index(int? page, int pageSize = 0)
        {
			var pageNumber = page == null || page <= 0 ? 1 : page.Value;
			var products = _productContext.findAll();
			if (pageSize == 0)
			{
				pageSize = Utilities.PAGE_SIZE_SHOP;
			}
			PagedList<Product> models = new PagedList<Product>(products.AsQueryable(), pageNumber, pageSize);
			return View(models);
        }


		[Route("/products/{alias}-{id}")]
		public IActionResult Details(int id,string alias)
        {
			ViewBag.Id = id;
			var product = _productContext.GetByIdAsync(id);
			ViewBag.variantId = product.ProductVariants.First().Id;
			var productsSimilar = _context.Products.Include(p => p.Category)
				   .Include(p => p.ProductImages)
				   .Include(p => p.ProductVariants)
				   .Include(p=>p.Ratings)
				   .Include(p => p.ProductMaterial).Where(x => x.CategoryId == product.CategoryId).Where(x=>x.ProductVariants.Count() >0).OrderByDescending(x=>x.Ratings.Any() ? x.Ratings.Average(r => r.NumberStars) : 0).ToList().Take(8);
			return View(productsSimilar);
        }

        [Route("/collections/{alias}")]
        public async Task<IActionResult> CollectionCategory(string alias, int? page = 0)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;
			var categories = await _categoryContext.GetAllAsync();

			var products = _productContext.findAll();
			var category = await _categoryContext.GetByAliasAsync(alias);
			IEnumerable<Product> productByAlias = new List<Product>();
            if (alias !=null || alias !="")
            {
                productByAlias = products.Where(x => x.ProductVariants.Count() > 0 && x.Category.Alias == alias || x.ProductCollectionId == category.Id || x.ProductObjectId == category.Id).ToList();
            }
            else
            {	
				productByAlias = products;

            }
            PagedList<Product> models = new PagedList<Product>(productByAlias.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
			ViewBag.Category = category;

            return View(models);

        }
        [Route("/collections/materials/{alias}")]
        public async Task<IActionResult> MaterialCategory(string alias, int? page = 0)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;

			var products = _productContext.findAll();
			var productMaterial = await _productMaterialContext.GetByAliasAsync(alias);
			var categories = await _categoryContext.GetAllAsync();
            List<Product> productByAlias = new List<Product>();
            if (alias != null || alias != "")
            {
				productByAlias = productMaterial.Products.Where(x=>x.ProductVariants.Count() >0).ToList();

			}
            else
            {
                productByAlias = products;

            }
            PagedList<Product> models = new PagedList<Product>(productByAlias.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
			ViewBag.Material = productMaterial;

			return View(models);

        }

        [HttpPost]
		[Route("/get-variant-price")]
		public IActionResult GetPriceAndVariantId(int variantId)
		{
			// Thực hiện logic để lấy giá và variantId mới dựa trên kích thước và productId
			var productVariant = _productVariantContext.GetById(variantId); // Thay thế bằng phương thức thích hợp của bạn

			if (productVariant != null)
			{
				// Trả về giá và variantId mới dưới dạng JSON
				var response = new
				{
					success = true,
					id = productVariant.Id,
					productName = productVariant.Product.Name,
					price = productVariant.Price,
					description = productVariant.Product.Description
					// Thêm các thông tin khác cần thiết tương ứng
				};
				return Json(response);
			}
			else
			{
				// Trả về lỗi nếu không tìm thấy product variant tương ứng
				return Json(new { success = false });
			}
		}
			
		[HttpPost]
		[Route("/submit-review")]
		public IActionResult SubmitReview(string userId, int productId, int numberStar, string message)
		{
			// Kiểm tra nếu userId là null
			if (string.IsNullOrEmpty(userId))
			{
				// Chuyển hướng đến trang đăng nhập
				return Json(new { success = false, message = "Bạn cần đăng nhập tài khoản." });
			}
			Rating rating = new Rating() {
				ProductId = productId,
				NumberStars = numberStar,
				Status = 1,
				Content = message,
				CreatedAt = DateTime.Now,
				UserId = userId
			};
			_context.Ratings.Add(rating);
			_context.SaveChanges();
			return Json(new { success = true, message = "Đánh giá của bạn đã được gửi thành công." });
		}


		[HttpGet]
		[Route("/accessories/filter-product-by-price")]
		public IActionResult FilterProductsByPrice(double minPrice, double maxPrice)
		{
			var filteredProducts = _productContext.findAll()
			  .Where(p => p.ProductVariants.First().Price >= minPrice && p.ProductVariants.First().Price <= maxPrice)
			  .ToList();

			return PartialView("_ProductListPartial", filteredProducts);
		}

		[HttpGet]
		[Route("/accessories/filter-product-by-option")]
		public IActionResult FilterProductsByOption(string option)
		{
			var filteredProducts = new List<Product>();
			if (option == "price")
			{
				filteredProducts = _productContext.findAll().OrderByDescending(x=>x.ProductVariants.First().Price)
			  .ToList();

			}else if(option == "rating")
			{
				filteredProducts = _productContext.findAll()
								.Select(p => new
								{
									Product = p,
									AverageRating = p.Ratings.Any() ? p.Ratings.Average(r => r.NumberStars) : 0
								})
								.OrderByDescending(x => x.AverageRating)
								.Select(x => x.Product)
								.ToList();
			}
			else
			{
				filteredProducts = _productContext.findAll();
			}
			

			return PartialView("_ProductListPartial", filteredProducts);
		}
	}
}
