using Accessories_Store.Areas.Admin.Services;
using Accessories_Store.Data_Access;
using Accessories_Store.Helpers;
using Accessories_Store.Models;
using Accessories_Store.Models.Entities;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace Accessories_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AdminProductController : Controller
    {
        private readonly IProductVariantRepo _productVariantContext;
        private readonly IProductRepo _productContext;
        private readonly ICategoryRepo _categoryContext;
        private readonly ApplicationDbContext _context;



        public INotyfService _notifyService { get; }
        public AdminProductController(ApplicationDbContext context, INotyfService notyfService, IProductRepo productContext, ICategoryRepo categoryContext, IProductVariantRepo productVariantContext)
        {
            _context = context;
            _notifyService = notyfService;
            _productContext = productContext;
            _categoryContext = categoryContext;
            _productVariantContext = productVariantContext;
        }

        [Route("/admin/products")]
        public async Task<IActionResult> Index(int? page = 0, int catID = 0, int productObjectId =0, int productCollectionId =0)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;

            IEnumerable<Product> lsProducts = new List<Product>();
            lsProducts = await _productContext.GetAllAsync();
            if (catID != 0)
            {
                lsProducts = await _productContext.GetAllByCatAsync(catID);
            }
            if(productObjectId != 0)
            {
                lsProducts = lsProducts.Where(p => p.ProductObjectId == productObjectId);
            }

            if(productCollectionId != 0)
            {
                lsProducts = lsProducts.Where(p => p.ProductCollectionId == productCollectionId);
            }
            
            PagedList<Product> models = new PagedList<Product>(lsProducts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentCatID = catID;

            ViewData["DanhMuc"] = new SelectList(_context.Categories.Where(x=>x.TypeId == CategoryTypeStatus.CategoryStatus), "Id", "Name", catID);
            ViewData["BoSuuTap"] = new SelectList(_context.Categories.Where(x=>x.TypeId == CategoryTypeStatus.CollectionStatus), "Id", "Name", catID);
            ViewData["DoiTuong"] = new SelectList(_context.Categories.Where(x => x.TypeId == CategoryTypeStatus.ObjectStatus), "Id", "Name", catID);


            ViewBag.ListProductVariant = _context.ProductVariants.ToList();
            return View(models);
        }

        // GET: Admin/AdminProducts/Create
        [Route("/admin/products/add")]
        public IActionResult Create()
        {
            ViewData["DanhMuc"] = new SelectList(_context.Categories.Where(x => x.TypeId == CategoryTypeStatus.CategoryStatus), "Id", "Name");
            ViewData["BoSuuTap"] = new SelectList(_context.Categories.Where(x => x.TypeId == CategoryTypeStatus.CollectionStatus), "Id", "Name");
            ViewData["DoiTuong"] = new SelectList(_context.Categories.Where(x => x.TypeId == CategoryTypeStatus.ObjectStatus), "Id", "Name");
            ViewData["ChatLieu"] = new SelectList(_context.ProductMaterials, "Id", "Name");

            return View();
        }
        // Xử lý thêm sản phẩm mới
        [HttpPost]
        [Route("/admin/products/add")]
        public async Task<IActionResult> Create(Product product, IFormFile? fThumb, List<IFormFile>? images)
        {

            if (ModelState.IsValid)
            {
                if (fThumb != null)
                {

                    string extention = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(product.Name) + "-" + product.Id + extention;
                    product.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                }
                if (images != null)
                {
                    product.ProductImages = new List<ProductImage>();
                    int i = 1;
                    foreach (var item in images)
                    {

                        string extention = Path.GetExtension(item.FileName);
                        string image = Utilities.SEOUrl(product.Name) + "-" + product.Id + "-" + i + extention;
                        ProductImage imagePro = new ProductImage()
                        {
                            ProductId = product.Id,
                            Path = await Utilities.UploadFile(item, @"products/product-details", image.ToLower())
                        };
                        i++;
                        product.ProductImages.Add(imagePro);
                    }
                }

                if (string.IsNullOrEmpty(product.Thumb)) product.Thumb = "default.jpg";

                product.Status = 1;
                product.Alias = Utilities.SEOUrl(product.Name);
                product.CreatedAt = DateTime.Now;
                product.UpdatedAt = DateTime.Now;


                await _productContext.AddAsync(product);

                _notifyService.Success("Tạo mới sản phẩm thành công!");
                return RedirectToAction(nameof(Index));
            }
            _notifyService.Error("Tạo mới sản phẩm thất bại!");
            ViewData["DanhMuc"] = new SelectList(_context.Categories.Where(x => x.TypeId == CategoryTypeStatus.CategoryStatus), "Id", "Name");
            ViewData["BoSuuTap"] = new SelectList(_context.Categories.Where(x => x.TypeId == CategoryTypeStatus.CollectionStatus), "Id", "Name", product.ProductCollectionId);
            ViewData["DoiTuong"] = new SelectList(_context.Categories.Where(x => x.TypeId == CategoryTypeStatus.ObjectStatus), "Id", "Name", product.ProductObjectId);
            ViewData["ChatLieu"] = new SelectList(_context.ProductMaterials, "Id", "Name");
            return View(product);
        }

        // GET: Admin/AdminProducts/Edit/5
        [Route("/admin/products/{id}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }

            var product =  _productContext.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories.Where(x => x.TypeId == CategoryTypeStatus.CategoryStatus), "Id", "Name", product.Category.Id);
            ViewData["BoSuuTap"] = new SelectList(_context.Categories.Where(x => x.TypeId == CategoryTypeStatus.CollectionStatus), "Id", "Name", product.ProductCollectionId);
            ViewData["DoiTuong"] = new SelectList(_context.Categories.Where(x => x.TypeId == CategoryTypeStatus.ObjectStatus), "Id", "Name", product.ProductObjectId);
            ViewData["ChatLieu"] = new SelectList(_context.ProductMaterials, "Id", "Name");
            ViewBag.ItemList = await _productVariantContext.GetAllByProductIdAsync(id);

            return View(product);
        }

        // POST: Admin/AdminProducts/Edit/5

        [HttpPost]
        [Route("/admin/products/{id}/edit")]

        public async Task<IActionResult> Edit(int id, Product product, IFormFile? fThumb, List<IFormFile> images)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (fThumb != null)
                    {
                        string extention = Path.GetExtension(fThumb.FileName);
                        string image = Utilities.SEOUrl(product.Name) + "-" +product.Id + extention;
                        product.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(product.Thumb)) product.Thumb = "default.jpg";

                    if (images != null)
                    {
                        product.ProductImages = new List<ProductImage>();
                        int i = 1;
                        foreach (var item in images)
                        {
                            
                            string extention = Path.GetExtension(item.FileName);
                            string image = Utilities.SEOUrl(product.Name) + "-" + product.Id + "-" + i + extention;
                            ProductImage imagePro = new ProductImage()
                            {
                                ProductId = product.Id,
                                Path = await Utilities.UploadFile(item, @"products/product-details", image.ToLower())
                            };
                            i++;
                            product.ProductImages.Add(imagePro);
                        }
                    }

                    product.Alias = Utilities.SEOUrl(product.Name);
                    

                    product.UpdatedAt = DateTime.Now;

                    await _productContext.UpdateAsync(product);
                    _notifyService.Success("Cập nhật sản phẩm thành công!");

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["BoSuuTap"] = new SelectList(_context.Categories.Where(x => x.TypeId == CategoryTypeStatus.CollectionStatus), "Id", "Name", product.ProductCollectionId);
            ViewData["DoiTuong"] = new SelectList(_context.Categories.Where(x => x.TypeId == CategoryTypeStatus.ObjectStatus), "Id", "Name", product.ProductObjectId);
            ViewData["ChatLieu"] = new SelectList(_context.ProductMaterials, "Id", "Name");
            _notifyService.Error("Cập nhật sản phẩm thất bại!");
            return View(product);
        }

        // GET: Admin/products/Delete/5
        [Route("/admin/products/{id}/delete")]
        public IActionResult Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product =  _productContext.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["ChatLieu"] = new SelectList(_context.ProductMaterials, "Id", "Name");
            return View(product);
        }

        // POST: Admin/AdminProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("/admin/products/{id}/delete")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'DbEcommerceMarketContext.Products'  is null.");
            }
            await _productContext.DeleteAsync(id);
            
            _notifyService.Success("Xóa sản phẩm thành công!");
            return RedirectToAction(nameof(Index));
        }

		[HttpPost]
		[Route("/admin/products/delete-image")]
		public async Task<IActionResult> DeleteImage(int id)
		{
			var image = await _context.ProductImages.FindAsync(id);
			var idTmp = image.ProductId;
			if (image == null)
				return NotFound();
			_context.ProductImages.Remove(image);
			await _context.SaveChangesAsync();
			return RedirectToAction("Edit", "AdminProduct", new { id = idTmp });
		}
		private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [Route("/admin/products/filter")]
        public IActionResult Filter(int CatID = 0)
		{

			var url = $"/admin/products?catID={CatID}";
			if (CatID == 0)
			{
				url = $"/admin/products";

			}
			return Json(new { status = "success", redirectUrl = url });
		}

        [Route("/admin/products/filter-collections")]
        public IActionResult FilterCollections(int productCollectionId = 0)
        {

            var url = $"/admin/products?productCollectionId={productCollectionId}";
            if (productCollectionId == 0)
            {
                url = $"/admin/products";

            }
            return Json(new { status = "success", redirectUrl = url });
        }
        [Route("/admin/products/filter-object")]
        public IActionResult FilterObject(int productObjectId = 0)
        {

            var url = $"/admin/products?productObjectId={productObjectId}";
            if (productObjectId == 0)
            {
                url = $"/admin/products";

            }
            return Json(new { status = "success", redirectUrl = url });
        }
    }


}
