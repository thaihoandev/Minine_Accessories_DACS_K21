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
using System.Collections.Generic;

namespace Accessories_Store.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class AdminBlogController : Controller
	{
		private readonly IBlogRepo _blogContext;
		private readonly ApplicationDbContext _context;

		public INotyfService _notifyService { get; }
		public AdminBlogController(ApplicationDbContext context, INotyfService notyfService, IBlogRepo blogContext)
		{
			_context = context;
			_notifyService = notyfService;
			_blogContext = blogContext;
		}

		[Route("/admin/blogs")]
		// GET: Admin/blogs
		public async Task<IActionResult> Index(int? page)
		{
			var pageNumber = page == null || page <= 0 ? 1 : page.Value;
			var pageSize = Utilities.PAGE_SIZE;
            IEnumerable<Blog> lsBlogs = new List<Blog>();
			lsBlogs = await _blogContext.GetAllAsync();
			PagedList<Blog> models = new PagedList<Blog>(lsBlogs.AsQueryable(), pageNumber, pageSize);
			ViewBag.CurrentPage = pageNumber;


			return View(models);
		}


		// GET: Admin/blogs/Create
		[Route("/admin/blogs/add")]
		public IActionResult Create()
		{

			return View();
		}

		// POST: Admin/blogs/Create
		[HttpPost]
		[Route("/admin/blogs/add")]
		public async Task<IActionResult> Create(Blog blog, IFormFile? fThumb)
		{
			if (ModelState.IsValid)
			{
				if (fThumb != null)
				{
					string extention = Path.GetExtension(fThumb.FileName);
					string image = Utilities.SEOUrl(blog.Title) + extention;
					blog.Thumb = await Utilities.UploadFile(fThumb, @"blogs", image.ToLower());
				}
				if (string.IsNullOrEmpty(blog.Thumb)) blog.Thumb = "default.jpg";

				blog.Alias = Utilities.SEOUrl(blog.Title);
				blog.CreatedAt = DateTime.Now;
				blog.UpdatedAt = DateTime.Now;
				blog.Status = Status.StatusOk;
				await _blogContext.AddAsync(blog);
				_notifyService.Success("Tạo mới thành công!");

				return RedirectToAction(nameof(Index));
			}
			_notifyService.Success("Tạo mới thất bại!");
			
			return View(blog);
		}

		// GET: Admin/blogs/Edit/5
		[Route("/admin/blogs/{id}/edit")]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.Blogs == null)
			{
				return NotFound();
			}

			var blog = await _blogContext.GetByIdAsync(id);
			if (blog == null)
			{
				return NotFound();
			}
	

			return View(blog);
		}

		// POST: Admin/blogs/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[Route("/admin/blogs/{id}/edit")]
		public async Task<IActionResult> Edit(int id, Blog blog, Microsoft.AspNetCore.Http.IFormFile? fThumb)
		{
			if (id != blog.Id)
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
						string image = Utilities.SEOUrl(blog.Title) + extention;
						blog.Thumb = await Utilities.UploadFile(fThumb, @"blogs", image.ToLower());
					}
					if (string.IsNullOrEmpty(blog.Thumb)) blog.Thumb = "default.jpg";

					blog.Alias = Utilities.SEOUrl(blog.Title);

                    blog.UpdatedAt = DateTime.Now;

					await _blogContext.UpdateAsync(blog);
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!BlogExists(blog.Id))
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

			return View(blog);
		}

		// GET: Admin/blogs/Delete/5
		[Route("/admin/blogs/{id}/delete")]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.Blogs == null)
			{
				return NotFound();
			}

			var blog = await _blogContext.GetByIdAsync(id);
			if (blog == null)
			{
				return NotFound();
			}

			return View(blog);
		}

		// POST: Admin/blogs/Delete/5
		[HttpPost, ActionName("Delete")]
		[Route("/admin/blogs/{id}/delete")]

		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.Categories == null)
			{
				return Problem("Entity set 'ApplicationDbContext.blogs'  is null.");
			}
			var blog = await _blogContext.GetByIdAsync(id);
			if (blog != null)
			{
				await _blogContext.DeleteAsync(id);
			}

			_notifyService.Success("Xóa danh mục thành công!");

			return RedirectToAction(nameof(Index));
		}

		private bool BlogExists(int id)
		{
			return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
		}

        [HttpPost]
        [Route("/admin/blogs/public-blog")]

        public async Task<IActionResult> Confirmed(int blogId)
        {
            var blog = _context.Blogs.FirstOrDefault(x => x.Id == blogId);
            if (blog == null)
            {
                return NotFound();
            }
            else
            {
                blog.Status = Status.StatusOk;
				blog.Published = true;
                await _blogContext.UpdateAsync(blog);
                _notifyService.Success("Confirm Success!");
                return RedirectToAction("Index");
            }
        }
    }
}
