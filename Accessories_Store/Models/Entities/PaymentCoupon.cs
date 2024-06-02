using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Accessories_Store.Data_Access;
using Accessories_Store.Helpers;

namespace Accessories_Store.Models.Entities
{
    public partial class PaymentCoupon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? CouponCode { get; set; }

        public string? Description { get; set; }

        public int DiscountValue { get; set; }

        public int DiscountUnit { get; set; }

        public int MinimumOrder { get; set; }

        public double MaximumDiscount { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ValidUntil { get; set; }
        public int Status { get; set; }

        public static string GenerateUniqueCouponCode(ApplicationDbContext context)
        {
            string couponCode;
            do
            {
                couponCode = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            } while (context.PaymentCoupons.Any(c => c.CouponCode == couponCode));

            return couponCode;
        }

        public static async Task UpdateExpiredCouponsStatus(ApplicationDbContext context)
        {
            var expiredCoupons = context.PaymentCoupons
                .Where(c => c.ValidUntil < DateTime.Now && c.Status != Accessories_Store.Helpers.Status.StatusCancel)
                .ToList();

            foreach (var coupon in expiredCoupons)
            {
                coupon.Status = Accessories_Store.Helpers.Status.StatusCancel;
            }

            await context.SaveChangesAsync();
        }
    }
}
