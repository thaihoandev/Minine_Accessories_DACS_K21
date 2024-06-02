using Accessories_Store.Data_Access;
using Accessories_Store.Helpers;
using Accessories_Store.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Accessories_Store.Areas.Admin.Services
{
    public class EFVoucherRepo : IVoucherRepo
    {
        private readonly ApplicationDbContext _context;
        public EFVoucherRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<PaymentCoupon>> GetAllAsync()
        {
            return await _context.PaymentCoupons.ToListAsync();
        }

        public async Task<PaymentCoupon> GetByIdAsync(int? id)
        {
            return await _context.PaymentCoupons.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<PaymentCoupon> GetByVoucherAsync(string? couponCode)
        {
            return await _context.PaymentCoupons.FirstOrDefaultAsync(p => p.CouponCode == couponCode);
        }
        public async Task AddAsync(PaymentCoupon paymentCoupon)
        {
            _context.PaymentCoupons.Add(paymentCoupon);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PaymentCoupon paymentCoupon)
        {
            _context.PaymentCoupons.Update(paymentCoupon);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var paymentCoupon = await _context.PaymentCoupons.FindAsync(id);
            paymentCoupon.Status = Status.StatusDelete;
            await _context.SaveChangesAsync();
        }
    }
}
