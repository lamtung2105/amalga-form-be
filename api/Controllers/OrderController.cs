using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController(OrderService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetOrderRequest(string orderGroupNumber, string facilityCode, string patientVisibleId)
    {
        var html = await service.RenderOrderRequest(orderGroupNumber, facilityCode, patientVisibleId);
        return Content(html, "text/html");
    }
}
