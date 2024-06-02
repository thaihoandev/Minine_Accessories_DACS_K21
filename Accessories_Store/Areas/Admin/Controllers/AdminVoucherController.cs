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
   
    public class AdminVoucherController : Controller
    {
        private readonly IVoucherRepo _voucherContext;
        private readonly ApplicationDbContext _context;

        public INotyfService _notifyService { get; }
        public AdminVoucherController(ApplicationDbContext context, INotyfService notyfService, IVoucherRepo voucherContext)
        {
            _context = context;
            _notifyService = notyfService;
            _voucherContext = voucherContext;
        }
        [Route("/admin/vouchers")]
        // GET: Admin/AdminPaymentCoupons
        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;
            IEnumerable<PaymentCoupon> lsCats = new List<PaymentCoupon>();
            lsCats = await _voucherContext.GetAllAsync();
            PagedList<PaymentCoupon> models = new PagedList<PaymentCoupon>(lsCats.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;


            return View(models);
        }

        // GET: Admin/AdminPaymentCoupons/Create
        [Route("/admin/vouchers/add")]
        public IActionResult Create()
        {
            
            return View();
        }

        // POST: Admin/AdminPaymentCoupons/Create
        [HttpPost]
        [Route("/admin/vouchers/add")]
        public async Task<IActionResult> Create(PaymentCoupon paymentCoupon)
        {
            if (ModelState.IsValid)
            {
                paymentCoupon.CouponCode = PaymentCoupon.GenerateUniqueCouponCode(_context);
                paymentCoupon.CreatedAt = DateTime.Now;
                paymentCoupon.ValidUntil = DateTime.Now.AddMonths(1);
                paymentCoupon.Status = Status.StatusOk;
                await _voucherContext.AddAsync(paymentCoupon);
                _notifyService.Success("Tạo mới thành công!");

                return RedirectToAction(nameof(Index));
            }
            _notifyService.Success("Tạo mới thất bại!");
            
            return View(paymentCoupon);
        }

        // GET: Admin/AdminPaymentCoupons/Edit/5
        [Route("/admin/vouchers/{id}/edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PaymentCoupons == null)
            {
                return NotFound();
            }

            var paymentCoupon = await _voucherContext.GetByIdAsync(id);
            if (paymentCoupon == null)
            {
                return NotFound();
            }
            
            return View(paymentCoupon);
        }

        // POST: Admin/AdminPaymentCoupons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("/admin/vouchers/{id}/edit")]
        public async Task<IActionResult> Edit(int id, PaymentCoupon paymentCoupon)
        {
            if (id != paymentCoupon.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    

                    await _voucherContext.UpdateAsync(paymentCoupon);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentCouponExists(paymentCoupon.Id))
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

            return View(paymentCoupon);
        }

        private bool PaymentCouponExists(int id)
        {
            return (_context.PaymentCoupons?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
