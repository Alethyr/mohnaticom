using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
  private readonly StoreContext _DbContext;
  public ProductsController(StoreContext DbContext)
  {
        _DbContext = DbContext;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return await _DbContext.Products.ToListAsync();
    }
   
   [HttpGet("{id:int}")]
   public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _DbContext.Products.FindAsync(id);
        if(product is null) return NotFound();
        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        _DbContext.Products.Add(product);
        await _DbContext.SaveChangesAsync();
        return product;
    }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if(product.Id != id || !ProductExists(id))
        {
            return BadRequest("Cannot update this product");
        }
        _DbContext.Entry(product).State = EntityState.Modified;
        await _DbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await _DbContext.Products.FindAsync(id);
        if(product is null) return NotFound();
        _DbContext.Products.Remove(product); 
        await _DbContext.SaveChangesAsync();
        return NoContent();
    }

    private bool ProductExists(int id)
    {
        return _DbContext.Products.Any(x => x.Id == id);
    }
}
