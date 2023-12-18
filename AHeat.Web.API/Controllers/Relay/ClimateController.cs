using AHeat.Web.API.Data;
using AHeat.Web.API.Models;
using AHeat.Web.Shared;
using AHeat.Web.Shared.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AHeat.Web.API.Controllers.Relay;

[ApiController]
[Route("api/relay/[controller]")]
public class ClimateController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public ClimateController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Get: "api/relay/climate
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ClimateDeviceDto>>> GetClimateDevices()
    {
        var climateDevices = await _dbContext.ClimateDevices.OrderBy(p => p.Name).ToListAsync();
        return Ok(climateDevices.Select(x => new ClimateDeviceDto(x.ID, x.DeviceId, x.Name)).ToList());
    }

    [HttpPost]
    // Put: "api/relay/climate
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Shared.Authorization.Authorize(Permissions.ConfigureClimateDevices)]
    public async Task<ActionResult> UpdateClimateDevice(ClimateDeviceDto climateDeviceDto)
    {
        ClimateDevice? climateDevice = await _dbContext.ClimateDevices.FirstOrDefaultAsync(x => x.DeviceId == climateDeviceDto.DeviceId);

        if (climateDevice == null)
        {
            return Problem($"No climate device with DeviceId {climateDeviceDto.DeviceId} found.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");
        }

        climateDevice.Name = climateDeviceDto.Name;

        _dbContext.ClimateDevices.Update(climateDevice);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: "api/relay/climate
    [HttpDelete()]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Permissions.ConfigureClimateDevices)]
    public async Task<IActionResult> DeleteClimateDevice(string deviceId)
    {
        ClimateDevice? climateDevice = await _dbContext.ClimateDevices.FirstOrDefaultAsync(p => p.DeviceId == deviceId);
        if (climateDevice == null)
        {
            return Problem($"No climate device with DeviceId {deviceId} found.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");
        }

        _dbContext.ClimateDevices.Remove(climateDevice);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    // Get: "api/relay/climate/info
    [HttpGet("info")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    // [Authorize]
    public async Task<ActionResult<IEnumerable<ClimateDeviceInfo>>> GetClimateDeviceInfos()
    {
        List<ClimateDeviceInfo> climateDeviceInfos = new List<ClimateDeviceInfo>();
        foreach (var climateDevice in _dbContext.ClimateDevices)
        {
            var climateDeviceReading = await _dbContext.ClimateDeviceReadings.Where(x => x.ClimateDeviceID == climateDevice.ID).OrderByDescending(o => o.Time).Include(i => i.ClimateDevice).FirstOrDefaultAsync();
            if (climateDeviceReading != null)
            {
                climateDeviceInfos.Add(new ClimateDeviceInfo(climateDevice.DeviceId, climateDevice.Name, climateDeviceReading.Temperature, climateDeviceReading.Humidity, climateDeviceReading.Time));
            }
        }
        return Ok(climateDeviceInfos);
    }
}
