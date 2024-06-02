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
    public class AdminCategoryController : Controller
    {
        private readonly ICategoryRepo _categoryContext;
        private readonly ApplicationDbContext _context;

        public INotyfService _notifyService { get; }
        public AdminCategoryController(ApplicationDbContext context, INotyfService notyfService,ICategoryRepo categoryContext)
        {
            _context = context;
            _notifyService = notyfService;
            _categoryContext = categoryContext;
        }

        [Route("/admin/categories")]
        // GET: Admin/AdminCategories
        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;
            IEnumerable<Category> lsCats = new List<Category>();
            lsCats = await _categoryContext.GetAllAsync();
            PagedList<Category> models = new PagedList<Category>(lsCats.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;


            return View(models);
        }

        // GET: Admin/AdminCategories/Details/5
        [Route("/admin/categories/details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            if ( _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _categoryContext.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/AdminCategories/Create
        [Route("/admin/categories/add")]
        public IActionResult Create()
        {
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["KieuLoai"] = new SelectList(_context.CategoryTypes, "Id", "Name");

            return View();
        }

        // POST: Admin/AdminCategories/Create
        [HttpPost]
        [Route("/admin/categories/add")]
        public async Task<IActionResult> Create(Category category, IFormFile? fThumb)
        {
            if (ModelState.IsValid)
            {
                if (fThumb != null)
                {
                    string extention = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(category.Name) + extention;
                    category.Thumb = await Utilities.UploadFile(fThumb, @"categories", image.ToLower());
                }
                if (string.IsNullOrEmpty(category.Thumb)) category.Thumb = "default.jpg";

                category.Alias = Utilities.SEOUrl(category.Name);
                category.CreatedAt = DateTime.Now;
                category.UpdatedAt = DateTime.Now;

                await _categoryContext.AddAsync(category);
                _notifyService.Success("Tạo mới danh mục thành công!");

                return RedirectToAction(nameof(Index));
            }
            _notifyService.Success("Tạo mới danh mục thất bại!");
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["KieuLoai"] = new SelectList(_context.CategoryTypes, "Id", "Name");
            return View(category);
        }

        // GET: Admin/AdminCategories/Edit/5
        [Route("/admin/categories/edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _categoryContext.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories.Where(x=>x.Id != id), "Id", "Name",category.ParentId);
            ViewData["KieuLoai"] = new SelectList(_context.CategoryTypes, "Id", "Name", category.CategoryType);

            return View(category);
        }

        // POST: Admin/AdminCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("/admin/categories/edit/{id}")]
        public async Task<IActionResult> Edit(int id, Category category, Microsoft.AspNetCore.Http.IFormFile? fThumb)
        {
            if (id != category.Id)
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
                        string image = Utilities.SEOUrl(category.Name) + extention;
                        category.Thumb = await Utilities.UploadFile(fThumb, @"categories", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(category.Thumb)) category.Thumb = "default.jpg";

                    category.Alias = Utilities.SEOUrl(category.Name);

                    await _categoryContext.UpdateAsync(category);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            _notifyService.Success("Cập nhật danh mục thất bại!");

            return View(category);
        }

        // GET: Admin/AdminCategories/Delete/5
        [Route("/admin/categories/delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _categoryContext.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/AdminCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("/admin/categories/delete/{id}")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
            }
            var category = await _categoryContext.GetByIdAsync(id);
            if (category != null)
            {
                await _categoryContext.DeleteAsync(id);
            }

            _notifyService.Success("Xóa danh mục thành công!");

            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
