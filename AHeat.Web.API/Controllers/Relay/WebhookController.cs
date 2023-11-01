using Microsoft.AspNetCore.Mvc;

namespace AHeat.Web.API.Controllers.Relay;

[Route("api/relay/[controller]")]
public class WebhookController : ControllerBase
{

    [HttpGet]
    public IActionResult Get()
    {
        var mac = Request.Query.Where(x => x.Key == "mac").FirstOrDefault()!.Value;
        var onoff = Request.Query.Where(x => x.Key == "switch").FirstOrDefault()!.Value;
        return Ok();
    }

    [HttpPost]
    public IActionResult Post()
    {
        
        return Ok();
    }
}
