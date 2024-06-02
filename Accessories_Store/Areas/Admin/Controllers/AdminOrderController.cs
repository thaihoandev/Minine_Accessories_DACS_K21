using Accessories_Store.Data_Access;
using Accessories_Store.Helpers;
using Accessories_Store.Models.Entities;
using Accessories_Store.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;
using Accessories_Store.Areas.Admin.Services;

namespace Accessories_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AdminOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public INotyfService _notifyService { get; }
        private readonly IOrderRepo _orderContext;
        private readonly IProductRepo _productContext;

        public AdminOrderController(ApplicationDbContext context, INotyfService notyfService, IOrderRepo orderContext, IProductRepo productContext)
        {
            _context = context;
            _orderContext = orderContext;
            _notifyService = notyfService;
            _productContext = productContext;
        }
        [Route("/admin/orders")]
        public async Task<IActionResult> Index(int? page = 0)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;
            var lsOrders = await _orderContext.GetAllAsync();
            PagedList<Order> models = new PagedList<Order>(lsOrders.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;


            return View(models);
        }

        public async Task<IActionResult> Pending(int? page = 0)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;
            var lsOrders = await _orderContext.GetAllByStatusAsync(2);
            PagedList<Order> models = new PagedList<Order>(lsOrders, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;


            return View(models);
        }

        [HttpPost]
        [Route("/admin/orders/confirmed-order")]

        public async Task<IActionResult> Confirmed(int orderId)
        {
            var order = _context.Orders.FirstOrDefault(x => x.Id == orderId);
            if (order == null)
            {
                return NotFound();
            }
            else
            {
                order.Status = Status.StatusOk;
                await _orderContext.UpdateAsync(order);
                _notifyService.Success("Confirm Success!");
                return RedirectToAction("Index");
            }
        }
        // GET: Admin/AdminOrder/Details/5
        [Route("/admin/orders/{id}/details")]

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _orderContext.GetByIdAsync(id);
            ViewBag.OrderDetails = order.OrderDetails;

            if (order == null)
            {
                return NotFound();
            }


            return View(order);
        }

        // GET: Admin/AdminOrder/Delete/5


        // POST: Admin/AdminOrder/Delete/5
        [HttpGet, ActionName("Delete")]
        [Route("/admin/orders/{id}/delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'DbEcommerceMarketContext.Orders'  is null.");
            }
            var order = await _orderContext.GetByIdAsync(id);
            if (order != null)
            {
                order.Status = Status.StatusDelete;
                await _orderContext.UpdateAsync(order);
            }
            _notifyService.Success("Xóa đơn hàng thành công!");
            return RedirectToAction(nameof(Index));
        }

    }
}
