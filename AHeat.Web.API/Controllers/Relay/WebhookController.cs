using System.Globalization;
using AHeat.Web.API.Data;
using AHeat.Web.API.Hubs;
using AHeat.Web.API.Models;
using AHeat.Web.Shared;
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
        await _powerStausHub.Clients.All.SendAsync("StatusUpdate", mac.ToString(), onoff.ToString());
        return Ok();
    }

    [HttpGet("temp")]
    public async Task<ActionResult> GetTemp()
    {
        var id = Request.Query.Where(x => x.Key == "id").FirstOrDefault()!.Value;
        var hum = Request.Query.Where(x => x.Key == "hum").FirstOrDefault()!.Value;
        var temp = Request.Query.Where(x => x.Key == "temp").FirstOrDefault()!.Value;
        var ip = Request.Headers["X-Forwarded-For"].ToString();
        if (string.IsNullOrEmpty(ip))
        {
            ip = this.HttpContext.Connection.RemoteIpAddress!.ToString();
        }
        _logger.LogInformation("Temp Webhook received from {ip}", ip);
        _logger.LogInformation("Id: {Id} Temp: {temp} Hum: {hum}", id, temp, hum);
        var climateDeviceInfo = ToClimateDeviceInfo(id!, temp!, hum!);

        await _powerStausHub.Clients.All.SendAsync("ClimateUpdate", climateDeviceInfo);
        await UpdateClimate(climateDeviceInfo);
        return Ok();
    }

    private ClimateDeviceInfo ToClimateDeviceInfo(string id, string temp, string hum)
    {
        double? tempD;
        double? humD;
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
        return new ClimateDeviceInfo(id, id, tempD, humD, DateTime.UtcNow);
    }

    private Task UpdateClimate(ClimateDeviceInfo climateDeviceInfo)
    {
        try
        {
            var device = _dbContext.ClimateDevices.Where(x => x.DeviceId == climateDeviceInfo.DeviceId).Include(c => c.Readings).FirstOrDefault();
            if (device == null)
            {
                device = new ClimateDevice();
                device.DeviceId = climateDeviceInfo.DeviceId;
                device.Name = climateDeviceInfo.Name;
                device.Readings = new List<ClimateDeviceReading>();
                device.Readings.Add(new ClimateDeviceReading { Humidity = climateDeviceInfo.Humidity, Temperature = climateDeviceInfo.Temperature, Time = climateDeviceInfo.Time });
                _dbContext.ClimateDevices.Add(device);
            }
            else
            {
                device.Readings.Add(new ClimateDeviceReading { Humidity = climateDeviceInfo.Humidity, Temperature = climateDeviceInfo.Temperature, Time = climateDeviceInfo.Time });
                _dbContext.ClimateDevices.Update(device);
            }
            _dbContext.SaveChanges();
            _logger.LogInformation("Updated climate from Id: {DeviceId} with name: {Name}", climateDeviceInfo.DeviceId, climateDeviceInfo.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when Updating Climate.");
        }
        return Task.CompletedTask;
    }
}
