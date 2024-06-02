using Accessories_Store.Areas.Admin.Services;
using Accessories_Store.Data_Access;
using Accessories_Store.Helpers;
using Accessories_Store.Models.Entities;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;
using System.Security.Claims;

namespace Accessories_Store.Areas.User.Controllers
{
	[Area("User")]

	public class BlogController : Controller
	{

        private readonly ApplicationDbContext _context;
        private readonly IBlogRepo _blogContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public INotyfService _notifyService { get; }
        public BlogController(ApplicationDbContext context, INotyfService notyfService, IBlogRepo blogContext, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _blogContext = blogContext;
            _notifyService = notyfService;
            _userManager = userManager;
        }

        [Route("/blogs")]
		public async Task<IActionResult> Index(int? page)
		{
			var pageNumber = page == null || page <= 0 ? 1 : page.Value;
			var pageSize = Utilities.PAGE_SIZE;
			var lsBLogs = await _blogContext.GetAllAsync();

			PagedList<Blog> models = new PagedList<Blog>(lsBLogs.Where(x=>x.Published==true).AsQueryable(), pageNumber, pageSize);
			ViewBag.CurrentPage = pageNumber;

			return View(models);
		}
        [Route("/blogs/{alias}-{id}")]
        public IActionResult Details(int id,string alias)
        {
            var blog = _blogContext.GetById(id);
            
            return View(blog);
        }
        [HttpPost]
        [Route("/blogs/add-comment")]

        public async Task<IActionResult> AddComment(string userId,int blogId, string content)
        {
            // Assuming you have a method to get the user by ID
            var user = await _userManager.FindByIdAsync(userId);
            var blog = _blogContext.GetById(blogId);
            if (user != null && !string.IsNullOrEmpty(content))
            {
                var comment = new Comment
                {
                    ApplicationUser = user,
                    Blog = blog,
                    Content = content,
                    // Set other properties as needed
                };

                await _context.Comments.AddAsync(comment);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    userName = user.Name,
                    content = comment.Content
                });
            }

            return BadRequest("Invalid user or content.");
        }
    }
}
