using AHeat.Web.API.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AHeat.Web.API.Controllers.Relay;

[Route("api/relay/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly ILogger<WebhookController> _logger;
    private readonly IHubContext<PowerStatusHub> _powerStausHub;

    public WebhookController(ILogger<WebhookController> logger, IHubContext<PowerStatusHub> powerStausHub)
    {
        _logger = logger;
        _powerStausHub = powerStausHub;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var mac = Request.Query.Where(x => x.Key == "mac").FirstOrDefault()!.Value;
        var onoff = Request.Query.Where(x => x.Key == "switch").FirstOrDefault()!.Value;
        var ip = Request.Headers["X-Forwarded-For"].ToString();
        if (string.IsNullOrEmpty(ip))
        {
            ip = this.HttpContext.Connection.RemoteIpAddress!.ToString();
        }
        _logger.LogInformation("Webhook received from {ip}", ip);
        _logger.LogInformation("Mac: {mac} Status: {onoff}", mac, onoff);
        _powerStausHub.Clients.All.SendAsync("StatusUpdate", mac.ToString(), onoff.ToString() );
        return Ok();
    }
}
