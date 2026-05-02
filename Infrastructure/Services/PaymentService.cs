using Core.Entities;
using Core.Intrefaces;
using Stripe;

namespace Infrastructure.Services;

public class PaymentService(StripeClient client, ICartService cartService, 
    ICouponService couponService ,IUnitOfWork unit) : IPaymentService
{
    private readonly StripeClient _client = client;
    public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
    {
        var cart = await cartService.GetCartAsync(cartId) ?? 
            throw new Exception("Cart unavailable");

        long shippingPrice = await GetShippingPriceAsync(cart);
        
        await ValidateCartItemsInCartAsync(cart);

        long subtotal = CalculateSubTotal(cart);   
        if (cart.Coupon != null)
        {
           subtotal = await ApplyDiscountAsync(cart.Coupon!, subtotal);
        }
      

        long total = subtotal + shippingPrice;

        await CreateUpdatePaymentIntentAsync(cart, total);

        await cartService.SetCartAsync(cart);

        return cart;
    }
    private async Task CreateUpdatePaymentIntentAsync(ShoppingCart cart, long total)
    {

        PaymentIntent intent; 
        var service = _client.V1.PaymentIntents;
        if (string.IsNullOrEmpty(cart.PaymentIntentId))
        {
            var options = new PaymentIntentCreateOptions
            {
              Amount = total,
              Currency = "rub",
              PaymentMethodTypes = ["card"]
            };
            intent = await service.CreateAsync(options);
            cart.PaymentIntentId = intent.Id;
            cart.ClientSecret = intent.ClientSecret;
        }
        else
        {
            var options = new PaymentIntentUpdateOptions
            {
              Amount = total
            };
            intent = await service.UpdateAsync(cart.PaymentIntentId, options);
        }
    }

    private async Task<long> ApplyDiscountAsync(AppCoupon code, long amount)
    {
        AppCoupon? coupon = await couponService.GetCouponByIdAsync(code.CouponId);
        
        if(coupon is null) return amount;
        
        if (coupon.AmountOff.HasValue)
        {
            amount -= (long)coupon.AmountOff.Value * 100;
        }
        if (coupon.PercentOff.HasValue)
        {
            var discount = amount * (coupon.PercentOff.Value / 100m);
            amount -= (long)discount;
        }

        return amount;
    }
    
    private long CalculateSubTotal(ShoppingCart cart)
    {
        return (long)cart.Items.Sum(x => x.Quantity * (x.Price * 100));
    }
    private async Task ValidateCartItemsInCartAsync(ShoppingCart cart)
    {
        foreach (var item in cart.Items)
        {
            var productItem = await unit.Repository<Core.Entities.Product>().
                GetByIdAsync(item.ProductId) 
                    ?? throw new Exception("Product not found");
            if (item.Price != productItem.Price)
            {
                item.Price = productItem.Price;
            }
        }
    }

    private async Task<long> GetShippingPriceAsync(ShoppingCart cart)
    {
        if (cart.DeliveryMethodId.HasValue)
        {
            var deliveryMethod = await unit.Repository<DeliveryMethod>().
                GetByIdAsync((int)cart.DeliveryMethodId);
            if (deliveryMethod is null) throw new Exception("Delivery method not found");
            return (long)deliveryMethod.Price * 100;
        }
        else return 0;
    }

    public async Task<string> RefundPayment(string paymentIntentId)
    {
        var refundOptions = new RefundCreateOptions
        {
            PaymentIntent = paymentIntentId
        };
        

        var refundService = _client.V1.Refunds;
        var result = await refundService.CreateAsync(refundOptions);
        return result.Status; 
    }
}
