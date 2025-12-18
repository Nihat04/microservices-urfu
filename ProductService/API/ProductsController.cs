using Microsoft.AspNetCore.Mvc;

namespace ProductService.API;

[ApiController]
[Route("[controller]")]
public class ProductsController() : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Test() => Ok();
}
