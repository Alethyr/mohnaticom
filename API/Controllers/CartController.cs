using Core.Entities;
using Core.Intrefaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class CartController(ICartService cartService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ShoppingCart>> GetCartById(string id)
    {
        ShoppingCart? cart = await cartService.GetCartAsync(id);
        return Ok(cart ?? new ShoppingCart { Id = id });
    } 

    [HttpPost]
    public async Task<ActionResult<ShoppingCart>> UpdateCart(ShoppingCart cart)
    {
        ShoppingCart? updatedCart = await cartService.SetCartAsync(cart);
        return Ok(updatedCart is null ? BadRequest("Problem with cart") : updatedCart);
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteCart(string id)
    {
        bool isDeleted = await cartService.DeleteCartAsync(id);
        return isDeleted ? Ok() : BadRequest("Problem with deleting cart");
    }
}
