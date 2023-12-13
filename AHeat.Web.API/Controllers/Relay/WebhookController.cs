using System.Globalization;
using AHeat.Web.API.Data;
using AHeat.Web.API.Hubs;
using AHeat.Web.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AHeat.Web.API.Controllers.Relay;

[Route("api/relay/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly ILogger<WebhookController> _logger;
    private readonly IHubContext<PowerStatusHub> _powerStausHub;
    private readonly ApplicationDbContext _dbContext;

    public WebhookController(ILogger<WebhookController> logger, IHubContext<PowerStatusHub> powerStausHub, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _powerStausHub = powerStausHub;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult> Get()
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
        await _powerStausHub.Clients.All.SendAsync("StatusUpdate", mac.ToString(), onoff.ToString() );
        return Ok();
    }

    [HttpGet("temp")]
    public async Task<ActionResult> GetTemp()
    {
        var id = Request.Query.Where(x => x.Key == "id").FirstOrDefault()!.Value;
        var name = Request.Query.Where(x => x.Key == "name").FirstOrDefault()!.Value;
        var hum = Request.Query.Where(x => x.Key == "hum").FirstOrDefault()!.Value;
        var temp = Request.Query.Where(x => x.Key == "temp").FirstOrDefault()!.Value;
        var ip = Request.Headers["X-Forwarded-For"].ToString();
        if (string.IsNullOrEmpty(ip))
        {
            ip = this.HttpContext.Connection.RemoteIpAddress!.ToString();
        }
        _logger.LogInformation("Temp Webhook received from {ip}", ip);
        _logger.LogInformation("Id: {Id} Name: {name} Temp: {temp} Hum: {hum}", id, name, temp, hum);
        await _powerStausHub.Clients.All.SendAsync("ClimateUpdate", id.ToString(), name.ToString(), temp.ToString(), hum.ToString() );
        await UpdateClimate(id!, name!, temp!, hum!);
        return Ok();
    }

    private Task UpdateClimate(string id, string name, string temp, string hum)
    {
        double? tempD;
        double? humD;
        try
        {
            if (double.TryParse(temp, CultureInfo.InvariantCulture, out double outTemp))
            {
                tempD = outTemp;
            }
            else
            {
                tempD = null;
            }
            if (double.TryParse(hum, out double outHum))
            {
                humD = outHum;
            }
            else
            {
                humD = null;
            }
            var device = _dbContext.ClimateDevices.Where(x => x.DeviceId == id).Include(c => c.Readings).FirstOrDefault();
            if (device == null)
            {
                device = new ClimateDevice();
                device.DeviceId = id;
                device.Name = name;
                device.Readings = new List<ClimateDeviceReading>();
                device.Readings.Add(new ClimateDeviceReading { Humidity = humD, Temperature = tempD, Time = DateTime.UtcNow });
                _dbContext.ClimateDevices.Add(device);
            }
            else
            {
                device.Readings.Add(new ClimateDeviceReading { Humidity = humD, Temperature = tempD, Time = DateTime.UtcNow });
                _dbContext.ClimateDevices.Update(device);
            }
            _dbContext.SaveChanges();
            _logger.LogInformation("Updated climate from Id: {id} with name: {name}", id, name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when Updating Climate.");
        }
        return Task.CompletedTask;
    }
}
