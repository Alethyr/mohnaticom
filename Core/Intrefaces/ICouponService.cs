using Core.Entities;

namespace Core.Intrefaces;

public interface ICouponService
{
   Task<AppCoupon?> GetCouponFromPromoCodeAsync(string coupon);
   Task<AppCoupon?> GetCouponByIdAsync(string couponId);
}
