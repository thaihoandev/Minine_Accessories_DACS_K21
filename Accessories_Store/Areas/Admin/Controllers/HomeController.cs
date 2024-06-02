using Accessories_Store.Areas.Admin.Services;
using Accessories_Store.Data_Access;
using Accessories_Store.Helpers;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accessories_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly IOrderRepo _orderContext;
        private readonly ApplicationDbContext _context;

        public INotyfService _notifyService { get; }
        public HomeController(ApplicationDbContext context, INotyfService notyfService, IOrderRepo orderContext)
        {
            _context = context;
            _notifyService = notyfService;
            _orderContext = orderContext;
        }
        public async Task<IActionResult> Index()
        {
            var orderList = await _orderContext.GetAllAsync();

            var revenue = orderList.Sum(x => x.TotalPrice);
            var discount = orderList.Sum(x => x.TotalDiscount);
            var CODPayment = orderList.Where(x=>x.OrderStatusPayment == PaymentType.StatusCOD).Sum(x => x.TotalPrice - x.TotalDiscount);
            var WalletPayment = orderList.Where(x => x.OrderStatusPayment == PaymentType.StatusVNPAY).Sum(x => x.TotalPrice - x.TotalDiscount);

            ViewBag.Revenue = revenue;
            ViewBag.Discount = discount;
            ViewBag.CODPayment = CODPayment;
            ViewBag.WalletPayment = WalletPayment;


            return View();
        }
    }
}
