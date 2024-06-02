using Accessories_Store.Areas.Admin.Services;
using Accessories_Store.Data_Access;
using Accessories_Store.Helpers;
using Accessories_Store.Models;
using Accessories_Store.Models.Entities;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace Accessories_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AdminCategoryTypeController : Controller
    {
        private readonly ICategoryRepo _categoryContext;
        private readonly ApplicationDbContext _context;

        public INotyfService _notifyService { get; }
        public AdminCategoryTypeController(ApplicationDbContext context, INotyfService notyfService, ICategoryRepo categoryContext)
        {
            _context = context;
            _notifyService = notyfService;
            _categoryContext = categoryContext;
        }

        [Route("/admin/categories/type")]
        // GET: Admin/AdminCategories
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;
            IEnumerable<CategoryType> lsCats = new List<CategoryType>();
            lsCats = _context.CategoryTypes.ToList();
            PagedList<CategoryType> models = new PagedList<CategoryType>(lsCats.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;


            return View(models);
        }


        // GET: Admin/AdminCategories/Create
        [Route("/admin/categories/type/add")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminCategories/Create
        [HttpPost]
        [Route("/admin/categories/type/add")]
        public IActionResult Create(CategoryType categoryType, IFormFile? fThumb)
        {
            if (ModelState.IsValid)
            {
                categoryType.CreatedAt = DateTime.Now;
                categoryType.UpdatedAt = DateTime.Now;

                _context.CategoryTypes.Add(categoryType);
                _context.SaveChanges();
                _notifyService.Success("Tạo mới thành công!");

                return RedirectToAction(nameof(Index));
            }
            _notifyService.Error("Tạo mới thất bại!");

            return View(categoryType);
        }

        // GET: Admin/AdminCategories/Edit/5
        [Route("/admin/categories/type/{id}/edit")]
        public IActionResult Edit(int? id)
        {
            if (id == null || _context.CategoryTypes == null)
            {
                return NotFound();
            }

            var categoryType = _context.CategoryTypes.FirstOrDefault(x => x.Id == id);
            if (categoryType == null)
            {
                return NotFound();
            }
            return View(categoryType);
        }

        // POST: Admin/AdminCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("/admin/categories/type/{id}/edit")]
        public IActionResult Edit(CategoryType categoryType,IFormFile? fThumb)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoryType);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryTypeExists(categoryType.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                _notifyService.Success("Cập nhật thành công!");

                return RedirectToAction(nameof(Index));
            }
            _notifyService.Error("Cập nhật thất bại!");

            return View(categoryType);
        }

        // GET: Admin/AdminCategories/Delete/5
        [Route("/admin/categories/type/{id}/delete")]
        public IActionResult Delete(int? id)
        {
            if (id == null || _context.CategoryTypes == null)
            {
                return NotFound();
            }

            var categoryType = _context.CategoryTypes.FirstOrDefault(x => x.Id == id);
            if (categoryType == null)
            {
                return NotFound();
            }

            return View(categoryType);
        }

        // POST: Admin/AdminCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("/admin/categories/delete/{id}")]

        public IActionResult DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
            }
            var categoryType = _context.CategoryTypes.FirstOrDefault(x=>x.Id == id);
            if (categoryType != null)
            {
                categoryType.Status = -1;
            }

            _notifyService.Success("Xóa thành công!");

            return RedirectToAction(nameof(Index));
        }

        private bool CategoryTypeExists(int id)
        {
            return (_context.CategoryTypes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
