using Microsoft.AspNetCore.Mvc;

namespace ProductService.Api;

[ApiController]
[Route(ApiConstants.ApiRoute)]
public class ProductsController() : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Test() => Ok();
}
