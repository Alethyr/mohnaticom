using API.RequestHelpers;
using Core.Entities;
using Core.Intrefaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IUnitOfWork unit) : BaseApiController
{
    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    { 
        var spec = new BrandListSpec();
        return Ok(await unit.Repository<Product>().ListAsync(spec));
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        var spec = new TypeListSpec();
        return Ok(await unit.Repository<Product>().ListAsync(spec));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(
      [FromQuery] ProductSpecParams specParams)
    {
        var spec = new ProductSpecification(specParams);
        return await CreatePagedResult(unit.Repository<Product>(), spec, specParams.PageIndex, specParams.PageSize);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await unit.Repository<Product>().GetByIdAsync(id);
        if (product is null) return NotFound();
        return product;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        unit.Repository<Product>().Add(product);
        if (await unit.Complete())
        {
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }
        return BadRequest("Problem creating Product");
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id || !ProductExists(id))
        {
            return BadRequest("Cannot update this product");
        }
        unit.Repository<Product>().Update(product);
        if (await unit.Complete())
        {
            return NoContent();
        }
        return BadRequest("Problem updating Product");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await unit.Repository<Product>().GetByIdAsync(id);
        if (product is null) return NotFound();
        unit.Repository<Product>().Remove(product);
        if (await unit.Complete())
        {
            return NoContent();
        }
        return BadRequest("Problem deleting problem");
    }

    private bool ProductExists(int id)
    {
        return unit.Repository<Product>().Exists(id);
    }
}
