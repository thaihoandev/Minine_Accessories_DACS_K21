using Accessories_Store.Areas.Admin.Services;
using Accessories_Store.Data_Access;
using Accessories_Store.Helpers;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Word;

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
        public async Task<IActionResult> Index(int year = 0, int month = 0)
        {
            year = year == 0 ? DateTime.Now.Year : year;
            month = month == 0 ? DateTime.Now.Month : month;

            var currentYear = year;
            var currentMonth = month;
            int previousMonth, previousMonthYear;
            if (currentMonth == 1)
            {
                previousMonth = 12;
                previousMonthYear = currentYear - 1;
            }
            else
            {
                previousMonth = currentMonth - 1;
                previousMonthYear = currentYear;
            }

            var orderList = await _orderContext.GetAllAsync();

            var monthRevenue = orderList.Where(x => x.CreatedAt.Value.Year == currentYear && x.CreatedAt.Value.Month == currentMonth).Sum(x => x.TotalPrice - x.TotalDiscount);
            var monthDiscount = orderList.Where(x =>x.CreatedAt.Value.Year == currentYear && x.CreatedAt.Value.Month == currentMonth).Sum(x => x.TotalDiscount);
            var monthCODPayment = orderList.Where(x=>x.OrderStatusPayment == PaymentType.StatusCOD && x.CreatedAt.Value.Year == currentYear && x.CreatedAt.Value.Month == currentMonth).Sum(x => x.TotalPrice - x.TotalDiscount);
            var monthWalletPayment = orderList.Where(x => x.OrderStatusPayment == PaymentType.StatusVNPAY && x.CreatedAt.Value.Year == currentYear && x.CreatedAt.Value.Month == currentMonth).Sum(x => x.TotalPrice - x.TotalDiscount);

            var yearPrevRevenue = orderList.Where(x => x.CreatedAt.Value.Year == currentYear-1).Sum(x => x.TotalPrice);

            var yearRevenue = orderList.Where(x => x.CreatedAt.Value.Year == currentYear ).Sum(x => x.TotalPrice - x.TotalDiscount);
			var yearDiscount = orderList.Where(x => x.CreatedAt.Value.Year == currentYear).Sum(x => x.TotalDiscount);
			var yearCODPayment = orderList.Where(x => x.OrderStatusPayment == PaymentType.StatusCOD && x.CreatedAt.Value.Year == currentYear ).Sum(x => x.TotalPrice - x.TotalDiscount);
			var yearWalletPayment = orderList.Where(x => x.OrderStatusPayment == PaymentType.StatusVNPAY && x.CreatedAt.Value.Year == currentYear ).Sum(x => x.TotalPrice - x.TotalDiscount);

            var monthPrevRevenue = orderList.Where(x =>x.CreatedAt.Value.Year == currentYear && x.CreatedAt.Value.Month == previousMonth).Sum(x => x.TotalPrice - x.TotalDiscount);
            var monthPrevDiscount = orderList.Where(x => x.CreatedAt.Value.Year == currentYear && x.CreatedAt.Value.Month == previousMonth).Sum(x => x.TotalDiscount);
            var monthPrevCODPayment = orderList.Where(x => x.OrderStatusPayment == PaymentType.StatusCOD && x.CreatedAt.Value.Year == currentYear && x.CreatedAt.Value.Month == previousMonth).Sum(x => x.TotalPrice - x.TotalDiscount);
            var monthPrevWalletPayment = orderList.Where(x => x.OrderStatusPayment == PaymentType.StatusVNPAY && x.CreatedAt.Value.Year == currentYear && x.CreatedAt.Value.Month == previousMonth).Sum(x => x.TotalPrice - x.TotalDiscount);

            ViewBag.orderList = orderList;
            ViewBag.orderListMonthCoupon = orderList.Where(x => x.CreatedAt.Value.Year == currentYear && x.CreatedAt.Value.Month == currentMonth && x.TotalDiscount > 0).Count();
            ViewBag.orderListMonth = orderList.Where(x => x.CreatedAt.Value.Year == currentYear && x.CreatedAt.Value.Month == currentMonth).ToList();

            ViewBag.yearPrevRevenue = yearPrevRevenue;

            ViewBag.yearRevenue = yearRevenue;
            ViewBag.yearDiscount = yearDiscount;
            ViewBag.yearCODPayment = yearCODPayment;
            ViewBag.yearWalletPayment = yearWalletPayment;

			ViewBag.monthRevenue = monthRevenue;
			ViewBag.monthDiscount = monthDiscount;
			ViewBag.monthCODPayment = monthCODPayment;
			ViewBag.monthWalletPayment = monthWalletPayment;

            ViewBag.monthPrevRevenue = monthPrevRevenue;
            ViewBag.monthPrevDiscount = monthPrevDiscount;
            ViewBag.monthPrevCODPayment = monthPrevCODPayment;
            ViewBag.monthPrevWalletPayment = monthPrevWalletPayment;

            return View();
        }
    }
}
