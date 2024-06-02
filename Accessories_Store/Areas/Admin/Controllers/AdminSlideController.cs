using Accessories_Store.Data_Access;
using Accessories_Store.Helpers;
using Accessories_Store.Models;
using Accessories_Store.Models.Entities;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace Accessories_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AdminSlideController : Controller
    {
        private readonly ApplicationDbContext _context;

        public INotyfService _notifyService { get; }
        public AdminSlideController(ApplicationDbContext context ,INotyfService notyfService)
        {
            _context = context;
            _notifyService = notyfService;
        }

        [Route("/admin/slides")]
        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;
            IEnumerable<Slide> lsSlides = new List<Slide>();
            lsSlides = await _context.Slides.ToListAsync();
            PagedList<Slide> models = new PagedList<Slide>(lsSlides.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;


            return View(models);
        }

        [Route("/admin/slides/add")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/slides/Create
        [HttpPost]
        [Route("/admin/slides/add")]
        public async Task<IActionResult> Create(Slide slide, IFormFile? fThumb)
        {
            if (ModelState.IsValid)
            {
                if (fThumb != null)
                {
                    string extention = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(slide.Name) + extention;
                    slide.Thumb = await Utilities.UploadFile(fThumb, @"slides", image.ToLower());
                }
                if (string.IsNullOrEmpty(slide.Thumb)) slide.Thumb = "default.jpg";

                slide.Status = 1;
                slide.CreatedAt = DateTime.Now;
                slide.UpdatedAt = DateTime.Now;

                await _context.Slides.AddAsync(slide);
                await _context.SaveChangesAsync();
                _notifyService.Success("Tạo mới thành công!");

                return RedirectToAction(nameof(Index));
            }
            _notifyService.Error("Tạo mới thất bại!");

            return View(slide);
        }

        // GET: Admin/AdminCategories/Edit/5
        [Route("/admin/slides/edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Slides == null)
            {
                return NotFound();
            }

            var slide = await _context.Slides.SingleOrDefaultAsync(x => x.Id == id);
            if (slide == null)
            {
                return NotFound();
            }
            return View(slide);
        }

        // POST: Admin/AdminCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("/admin/slides/edit/{id}")]
        public async Task<IActionResult> Edit(int id, Slide slide,IFormFile? fThumb)
        {
            if (id != slide.Id)
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
                        string image = Utilities.SEOUrl(slide.Name) + extention;
                        slide.Thumb = await Utilities.UploadFile(fThumb, @"slides", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(slide.Thumb)) slide.Thumb = "default.jpg";

                    slide.UpdatedAt = DateTime.Now;

                    _context.Slides.Update(slide);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SlideExists(slide.Id))
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
            _notifyService.Success("Cập nhật thất bại!");

            return View(slide);
        }
        private bool SlideExists(int id)
        {
            return (_context.Slides?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
