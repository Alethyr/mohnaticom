using Core.Entities;
using Core.Intrefaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class CouponsController(ICouponService couponService) : BaseApiController
{
   [HttpGet("{code}")]
   public async Task<ActionResult<AppCoupon>> ValidateCoupon(string code)
    {
        var coupon = await couponService.GetCouponFromPromoCodeAsync(code);
        if(coupon is null) return BadRequest("Invalid voucher code");
        return coupon;
    }
    
}
