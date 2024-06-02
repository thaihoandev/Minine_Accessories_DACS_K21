using Accessories_Store.Areas.Admin.Services;
using Accessories_Store.Data_Access;
using Accessories_Store.Models.Entities;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Accessories_Store.Areas.User.Controllers
{
    [Area("User")]
    [Route("home")]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IProductRepo _productRepo;

        public INotyfService _notifyService { get; }
        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, INotyfService notyfService, IProductRepo productRepo)
        {
            _context = context;
            _notifyService = notyfService;
            _userManager = userManager;
            _roleManager = roleManager;
            _productRepo = productRepo;
        }
        [Route("~/")]
		public IActionResult Index()
        {
            ViewBag.Slider = _context.Slides.ToList().Take(3);
            var productHome = _productRepo.findAll();
            ViewBag.ProductHome = productHome.Where(x=>x.Homeflag == true).Take(6);
			return View();

        }
    }
}
