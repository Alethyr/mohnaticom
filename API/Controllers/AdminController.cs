using API.DTOs;
using API.Extensions;
using Core.Entities.OrderAggregate;
using Core.Intrefaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(IUnitOfWork unit, IPaymentService paymentService) : BaseApiController
{
    [HttpGet("orders")]
    public async Task<ActionResult<IReadOnlyList<OrderDTO>>> GetOrders([FromQuery] OrderSpecParams specParams)
    {
        var spec = new OrderSpecification(specParams);
        return await CreatePagedResult(unit.Repository<Order>(), spec, specParams.PageIndex, specParams.PageSize, o => o.ToDTO());
    }

    [HttpGet("orders/{id:int}")]
    public async Task<ActionResult<OrderDTO>> GetOrderById(int id)
    {
        var spec = new OrderSpecification(id);
        var order = await unit.Repository<Order>().GetEntityWithSpec(spec);
        if(order is null) return BadRequest("No order with that ID");
        return order.ToDTO();
    }

    [HttpPost("orders/refund/{id:int}")]
    public async Task<ActionResult<OrderDTO>> RefundOrder(int id)
    {
        var spec = new OrderSpecification(id);
        var order = await unit.Repository<Order>().GetEntityWithSpec(spec);

        if(order is null) 
            return BadRequest("No order with that ID");

        if(order.Status == OrderStatus.Pending)
            return BadRequest("Payment not recieved for this order");

        var result = await paymentService.RefundPayment(order.PaymentIntentId);
        
        if(result == "succeeded")
        {
            order.Status = OrderStatus.Refunded;
            await unit.Complete();
            return order.ToDTO();
        }

        return BadRequest("Problem with refunding order");
        
    }
}

