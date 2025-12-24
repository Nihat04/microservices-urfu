using Common.Contracts;
using Common.Contracts.Products;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application;
using ProductService.Application.Extensions;
using ProductService.Application.Infrastructure;
using ProductService.Domain.Entities;

namespace ProductService.Api;

[ApiController]
[Route(ApiConstants.ApiRoute)]
public class ProductsController(IProductApplicationService productService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ProductResponse>> CreateProduct(
        ProductCreateRequest request,
        CancellationToken ct
    )
    {
        var result = await productService.CreateProduct(request, ct);
        return result.ToActionResult(this, p => p.ToDto());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetProductById(Guid id, CancellationToken ct)
    {
        var product = await productService.GetProductByIdAsync(id, ct);
        return product is null ? NotFound() : Ok(product.ToDto());
    }

    [HttpGet]
    public async Task<IActionResult> GetProductsByIds([FromQuery] Guid[] ids, CancellationToken ct)
    {
        if (ids.Length == 0)
            return Ok(Array.Empty<Product>());
        var products = await productService.GetProductsByIdsAsync(ids, ct);
        return Ok(products.Select(p => p!.ToDto()));
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetProducts(
        [FromQuery] Pagination pagination,
        [FromQuery] ProductSortBy sortBy = ProductSortBy.CreatedDate,
        [FromQuery] SortOrder sortOrder = SortOrder.Ascending,
        [FromQuery] ProductFilterDto? filter = default,
        CancellationToken ct = default
    )
    {
        var (products, count) = await productService.GetProductsAsync(
            pagination: pagination,
            sortBy: sortBy,
            sortOrder: sortOrder,
            filter: filter,
            ct
        );
        var productsDto = products.Select(p => p.ToDto()).ToArray();
        var response = new PaginatedResult<ProductResponse>(productsDto, pagination, count);
        return Ok(response);
    }

    [HttpPost("{id:guid}/book")]
    public async Task<ActionResult<ProductResponse>> BookProduct(
        Guid id,
        int quantity,
        CancellationToken ct
    )
    {
        var result = await productService.BookProduct(id, quantity, ct);
        return result.ToActionResult(this, p => p.ToDto());
    }

    [HttpPost("{id:guid}/cancel-book")]
    public async Task<ActionResult<ProductResponse>> CancelBookingProduct(
        Guid id,
        int quantity,
        CancellationToken ct
    )
    {
        var result = await productService.CancelBookingProduct(id, quantity, ct);
        return result.ToActionResult(this, p => p.ToDto());
    }

    [HttpPost("{id:guid}/add")]
    public async Task<ActionResult<ProductResponse>> AddProduct(
        Guid id,
        int quantity,
        CancellationToken ct
    )
    {
        var result = await productService.AddProduct(id, quantity, ct);
        return result.ToActionResult(this, p => p.ToDto());
    }

    [HttpPost("{id:guid}/ship")]
    public async Task<ActionResult<ProductResponse>> ShipProduct(
        Guid id,
        int quantity,
        CancellationToken ct
    )
    {
        var result = await productService.ShipProduct(id, quantity, ct);
        return result.ToActionResult(this, p => p.ToDto());
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> RemoveProduct(Guid id, CancellationToken ct)
    {
        var result = await productService.RemoveProduct(id, ct);
        return result.ToActionResult(this, p => p.ToDto());
    }
}
