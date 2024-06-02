using Accessories_Store.Areas.Admin.Services;
using Accessories_Store.Data_Access;
using Accessories_Store.Models.Entities;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Accessories_Store.Areas.User.Controllers
{
    [Area("User")]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlogRepo _blogContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public INotyfService _notifyService { get; }
        public AccountController(ApplicationDbContext context, INotyfService notyfService, IBlogRepo blogContext, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _blogContext = blogContext;
            _notifyService = notyfService;
            _userManager = userManager;
        }
       /* [Route("/account/order-history")]*/
        public IActionResult OrderHistory()
        {
            var orderList = _context.Orders.Where(x => x.ApplicationUser.Id == _userManager.GetUserId(User)).ToList();

            return View(orderList);
        }

        public IActionResult OrderDetail(int orderId)
        {
            var order = _context.Orders.Include(x=>x.ApplicationUser).Include(x=>x.OrderDetails).ThenInclude(x=>x.Product).Where(x => x.ApplicationUser.Id == _userManager.GetUserId(User) && x.Id == orderId).FirstOrDefault();

            return View(order);
        }
    }
}
