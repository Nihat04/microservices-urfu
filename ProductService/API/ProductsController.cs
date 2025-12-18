using Microsoft.AspNetCore.Mvc;
using ProductService.Application;
using ProductService.Domain.Entities;

namespace ProductService.Api;

[ApiController]
[Route(ApiConstants.ApiRoute)]
public class ProductsController(IProductApplicationService productService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductById(Guid id, CancellationToken ct)
    {
        var product = await productService.GetProductsByIdAsync(id, ct);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet]
    public async Task<IActionResult> GetProductsByIds([FromQuery] Guid[] ids, CancellationToken ct)
    {
        if (ids.Length == 0)
            return Ok(Array.Empty<Product>());
        var products = await productService.GetProductsByIdsAsync(ids, ct);
        return Ok(products);
    }
}
