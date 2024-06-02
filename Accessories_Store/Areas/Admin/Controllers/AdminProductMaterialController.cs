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
    public class AdminProductMaterialController : Controller
    {
        private readonly ICategoryRepo _categoryContext;
        private readonly IProductMaterialRepo _productMaterialContext;

        private readonly ApplicationDbContext _context;

        public INotyfService _notifyService { get; }
        public AdminProductMaterialController(ApplicationDbContext context, INotyfService notyfService,ICategoryRepo categoryContext, IProductMaterialRepo productMaterialContext)
        {
            _context = context;
            _notifyService = notyfService;
            _categoryContext = categoryContext;
            _productMaterialContext = productMaterialContext;
        }

        [Route("/admin/materials")]
        // GET: Admin/materials
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;
            var lsProductMaterials = _productMaterialContext.findAll();
            PagedList<ProductMaterial> models = new PagedList<ProductMaterial>(lsProductMaterials.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;


            return View(models);
        }

        // GET: admin/materials/add
        [Route("/admin/materials/add")]
        public IActionResult Create()
        {
            ViewData["DanhMuc"] = new SelectList(_context.ProductMaterials, "Id", "Name");
            return View();
        }

        // POST: admin/materials/add
        [HttpPost]
        [Route("/admin/materials/add")]
        public async Task<IActionResult> Create(ProductMaterial productMaterial, IFormFile? fThumb)
        {
            if (ModelState.IsValid)
            {
                if (fThumb != null)
                {
                    string extention = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(productMaterial.Name) + extention;
                    productMaterial.Thumb = await Utilities.UploadFile(fThumb, @"productMaterials", image.ToLower());
                }
                if (string.IsNullOrEmpty(productMaterial.Thumb)) productMaterial.Thumb = "default.jpg";

                productMaterial.Alias = Utilities.SEOUrl(productMaterial.Name);
                productMaterial.CreatedAt = DateTime.Now;
                productMaterial.UpdatedAt = DateTime.Now;

                await _productMaterialContext.AddAsync(productMaterial);
                _notifyService.Success("Tạo mới thành công!");

                return RedirectToAction(nameof(Index));
            }
            _notifyService.Error("Tạo mới thất bại!");

            return View(productMaterial);
        }

        // GET: Admin/materials/Edit/5
        [Route("/admin/materials/{id}/edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProductMaterials == null)
            {
                return NotFound();
            }

            var productMaterial = await _productMaterialContext.GetByIdAsync(id);
            if (productMaterial == null)
            {
                return NotFound();
            }
            return View(productMaterial);
        }

        // POST: admin/materials/edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("/admin/materials/{id}/edit")]
        public async Task<IActionResult> Edit(int id, ProductMaterial productMaterial, IFormFile? fThumb)
        {
            if (id != productMaterial.Id)
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
                        string image = Utilities.SEOUrl(productMaterial.Name) + extention;
                        productMaterial.Thumb = await Utilities.UploadFile(fThumb, @"productMaterials", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(productMaterial.Thumb)) productMaterial.Thumb = "default.jpg";

                    productMaterial.Alias = Utilities.SEOUrl(productMaterial.Name);

                    await _productMaterialContext.UpdateAsync(productMaterial);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductMaterialExists(productMaterial.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                _notifyService.Success("Cập nhật danh mục thành công!");

                return RedirectToAction(nameof(Index));
            }
            _notifyService.Error("Cập nhật danh mục thất bại!");

            return View(productMaterial);
        }

        // GET: Admin/materials/Delete/5
        [Route("/admin/materials/{id}/delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProductMaterials == null)
            {
                return NotFound();
            }

            var productMaterial = await _productMaterialContext.GetByIdAsync(id);
            if (productMaterial == null)
            {
                return NotFound();
            }

            return View(productMaterial);
        }

        // POST: Admin/materials/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("/admin/materials/{id}/delete")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductMaterials == null)
            {
                return Problem("Entity set 'ApplicationDbContext.productMaterials'  is null.");
            }
            var productMaterial = await _productMaterialContext.GetByIdAsync(id);
            if (productMaterial != null)
            {
                await _productMaterialContext.DeleteAsync(id);
            }

            _notifyService.Success("Xóa danh mục thành công!");

            return RedirectToAction(nameof(Index));
        }

        private bool ProductMaterialExists(int id)
        {
            return (_context.ProductMaterials?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
