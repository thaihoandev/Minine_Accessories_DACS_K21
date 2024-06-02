using Accessories_Store.Areas.Admin.Services;
using Accessories_Store.Data_Access;
using Accessories_Store.Helpers;
using Accessories_Store.Models;
using Accessories_Store.Models.Entities;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace Accessories_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AdminProductVariantController : Controller
    {
        private readonly IProductVariantRepo _productVariantContext;
        private readonly IProductRepo _productContext;

        private readonly ApplicationDbContext _context;

        public INotyfService _notifyService { get; }
        public AdminProductVariantController(ApplicationDbContext context, INotyfService notyfService, IProductVariantRepo productVariantContext, IProductRepo productContext)
        {
            _context = context;
            _notifyService = notyfService;
            _productVariantContext = productVariantContext;
            _productContext = productContext;
        }

        // GET: Admin/AdminProductVariant/Create
        [Route("/admin/products/{productId}/add-details")]
        public IActionResult Create(int productId)
        {
            ViewBag.Product = productId;
            return View();
        }

        // POST: Admin/AdminProductVariant/Create
        [HttpPost]
        [Route("/admin/products/{productId}/add-details")]
        public async Task<IActionResult> Create(ProductVariant productVariant, int productId)
        {
            if (ModelState.IsValid)
            {
                Product product =  _productContext.GetByIdAsync(productId);
                if (product != null)
                {
                    productVariant.CreatedAt = DateTime.Now;
                    productVariant.UpdatedAt = DateTime.Now;
                    // Thêm productVariant vào List<Products> của attribute
                    await _productContext.AddProductVariantAsync(product, productVariant);

                    _notifyService.Success("Tạo mới thành công!");
                    return RedirectToAction("edit","adminProduct", new { Id = productId });
                }
                else
                {
                    _notifyService.Error("Không tìm thấy sản phẩm.");
                    return RedirectToAction("edit","adminProduct", new { Id = productId });
                }
            }
            _notifyService.Success("Tạo mới thất bại!");

            return View(productVariant);
        }

        // GET: Admin/AdminCategories/Edit/5
        [Route("/admin/products/{productId}/edit-details/{id}")]
        public IActionResult Edit(int productId, int? id)
        {
            if (id == null || _context.ProductVariants == null)
            {
                return NotFound();
            }
            ViewBag.Product = productId;
            var productVariant = _productVariantContext.GetById(id);
            if (productVariant == null)
            {
                return NotFound();
            }
            return View(productVariant);
        }

        // POST: Admin/AdminCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("/admin/products/{productId}/edit-details/{id}")]
        public async Task<IActionResult> Edit(ProductVariant productVariant, int productId, int id)
        {
            if (id != productVariant.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (productVariant != null)
                    {
                        productVariant.UpdatedAt = DateTime.Now;
                        // Thêm productVariant vào List<Products> của attribute
                        await _productVariantContext.UpdateAsync(productVariant);

                        _notifyService.Success("Cập nhật thành công!");
                        return RedirectToAction("edit", "adminProduct", new { Id = productId });
                    }
                    else
                    {
                        _notifyService.Error("Không tìm thấy sản phẩm.");
                        return RedirectToAction("edit", "adminProduct", new { Id = productId });
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                        return NotFound();
                    
                }
            }
            _notifyService.Success("Cập nhật thất bại!");

            return View(productVariant);
        }

        // GET: Admin/AdminCategories/Delete/5
        [Route("/admin/products/{productId}/delete-details/{id}")]
        public IActionResult Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var productVariant = _productVariantContext.GetById(id);
            if (productVariant == null)
            {
                return NotFound();
            }

            return View(productVariant);
        }

        // POST: Admin/AdminCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("/admin/products/{productId}/delete-details/{id}")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
            }
            await _productVariantContext.DeleteAsync(id);
            

            _notifyService.Success("Xóa thành công!");

            return RedirectToAction(nameof(Index));
        }

      
    }
}
