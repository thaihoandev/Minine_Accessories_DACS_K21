using Accessories_Store.Models.Entities;

namespace Accessories_Store.Areas.Admin.Services
{
    public interface IVoucherRepo
    {

        Task<IEnumerable<PaymentCoupon>> GetAllAsync();
        Task<PaymentCoupon> GetByIdAsync(int? id);
        Task<PaymentCoupon> GetByVoucherAsync(string? couponCode);
        Task AddAsync(PaymentCoupon paymentCoupon);
        Task UpdateAsync(PaymentCoupon paymentCoupon);
        Task DeleteAsync(int id);
    }
}
