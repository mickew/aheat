using Microsoft.AspNetCore.Mvc;

namespace AHeat.Web.API.Controllers.Relay;

[Route("api/relay/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(ILogger<WebhookController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var mac = Request.Query.Where(x => x.Key == "mac").FirstOrDefault()!.Value;
        var onoff = Request.Query.Where(x => x.Key == "switch").FirstOrDefault()!.Value;
        _logger.LogInformation($"Webhook received from {Request.Host}");
        return Ok();
    }
}
