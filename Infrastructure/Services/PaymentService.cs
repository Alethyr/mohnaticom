using Core.Entities;
using Core.Intrefaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Services;

public class PaymentService(StripeClient client, ICartService cartService, IUnitOfWork unit) : IPaymentService
{
    private readonly StripeClient _client = client;
    public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
    {
        var cart = await cartService.GetCartAsync(cartId);

        if(cart is null) return null;

        var shippingPrice = 0m;

        if (cart.DeliveryMethodId.HasValue)
        {
            var deliveryMethod = await unit.Repository<DeliveryMethod>().
                GetByIdAsync((int)cart.DeliveryMethodId);
            if (deliveryMethod is null) return null;
            shippingPrice = deliveryMethod.Price;
        }

        foreach (var item in cart.Items)
        {
            var productItem = await unit.Repository<Core.Entities.Product>().
                GetByIdAsync(item.ProductId);
            if (productItem is null) return null;
            if (item.Price != productItem.Price)
            {
                item.Price = productItem.Price;
            }
        }

        var service = _client.V1.PaymentIntents;
        var amount = (long)cart.Items.Sum(x => x.Quantity * (x.Price * 100))  + (long)shippingPrice * 100;
        PaymentIntent intent;   

        if (string.IsNullOrEmpty(cart.PaymentIntentId))
        {
            var options = new PaymentIntentCreateOptions
            {
              Amount = amount,
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
              Amount = amount
            };
            intent = await service.UpdateAsync(cart.PaymentIntentId, options);
        }

        await cartService.SetCartAsync(cart);

        return cart;
    }
}
