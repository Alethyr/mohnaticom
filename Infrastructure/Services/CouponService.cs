using Core.Entities;
using Core.Intrefaces;
using Stripe;

namespace Infrastructure.Services;

public class CouponService(StripeClient client) : ICouponService
{
    private readonly StripeClient _client = client;

    public async Task<AppCoupon?> GetCouponByIdAsync(string couponId)
    {
        var service = _client.V1.Coupons;
        var coupon = await service.GetAsync(couponId);
        if(coupon is null) return null;
        return new AppCoupon
        {
          Name = coupon.Name,
          AmountOff = coupon.AmountOff,
          PercentOff = coupon.PercentOff,
          CouponId = coupon.Id,
          PromotionCode = null!
        };
    }

    public async Task<AppCoupon?> GetCouponFromPromoCodeAsync(string code)
    {
        var service = _client.V1.PromotionCodes;
        var options = new PromotionCodeListOptions
        {
            Code = code,
            Expand = ["data.promotion.coupon"]
        };

        var promotionCodes = await service.ListAsync(options);
        var promotionCode = promotionCodes.FirstOrDefault();

        if (promotionCode?.Promotion?.Coupon != null)
        {
            return new AppCoupon
            {
                Name = promotionCode.Promotion.Coupon.Name,
                AmountOff = promotionCode.Promotion.Coupon.AmountOff,
                PercentOff = promotionCode.Promotion.Coupon.PercentOff,
                CouponId = promotionCode.Promotion.Coupon.Id,
                PromotionCode = promotionCode.Code
            };
        }

        return null;
    }
}