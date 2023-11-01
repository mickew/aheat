using AHeat.Application.Exceptions;
using AHeat.Application.Services;
using AHeat.Web.API.Data;
using AHeat.Web.API.Models;
using AHeat.Web.Shared;
using AHeat.Web.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AHeat.Web.API.Controllers.Relay;

[ApiController]
[Route("api/relay/[controller]")]
public class PowerController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly StrategyDeviceService _strategyDeviceService;

    public PowerController(ApplicationDbContext dbContext, StrategyDeviceService strategyDeviceService)
    {
        _dbContext = dbContext;
        _strategyDeviceService = strategyDeviceService;
    }

    // Get: "api/Relay/Power
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PowerDto>>> GetPowerDevices()
    {
        var powerDevices = await _dbContext.PowerDevices.OrderBy(p => p.Name).ToListAsync();
        return powerDevices.Select(p => new PowerDto()
        {
            ID = p.ID,
            Name = p.Name,
            DeviceType = p.DeviceType,
            HostName = p.HostName,
            DeviceName = p.DeviceName,
            DeviceId = p.DeviceId,
            DeviceMac = p.DeviceMac,
            DeviceModel = p.DeviceModel,
            DeviceGen = p.DeviceGen,
        }).ToList();
    }

    // Get: "api/Relay/Power/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PowerDto>> GetPowerDevice(int id)
    {
        var powerDevice = await _dbContext.PowerDevices.FirstOrDefaultAsync(p => p.ID == id);
        if (powerDevice == null)
        {
            return Problem($"No device with id {id} found.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");
            //return NotFound();
        }

        return new PowerDto()
        {
            ID = powerDevice.ID,
            Name = powerDevice.Name,
            DeviceType = powerDevice.DeviceType,
            HostName = powerDevice.HostName,
            DeviceName = powerDevice.DeviceName,
            DeviceId = powerDevice.DeviceId,
            DeviceMac = powerDevice.DeviceMac,
            DeviceModel = powerDevice.DeviceModel,
            DeviceGen = powerDevice.DeviceGen,
        };
    }

    // post: "api/Relay/Power
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PowerDto>> PostPowerDevice(PowerDto newPowerDevice)
    {
        var powerDevice = new PowerDevice()
        {
            Name = newPowerDevice.Name,
            DeviceType = newPowerDevice.DeviceType,
            HostName = newPowerDevice.HostName,
            DeviceName = newPowerDevice.DeviceName,
            DeviceId = newPowerDevice.DeviceId,
            DeviceMac = newPowerDevice.DeviceMac,
            DeviceModel = newPowerDevice.DeviceModel,
            DeviceGen = newPowerDevice.DeviceGen,
        };

        var entety = await _dbContext.PowerDevices.AddAsync(powerDevice);
        await _dbContext.SaveChangesAsync();

        PowerDto powerDto = new PowerDto()
        {
            ID = entety.Entity.ID,
            Name = entety.Entity.Name,
            DeviceType = entety.Entity.DeviceType,
            HostName = entety.Entity.HostName,
            DeviceName = entety.Entity.DeviceName,
            DeviceId = entety.Entity.DeviceId,
            DeviceMac = entety.Entity.DeviceMac,
            DeviceModel = entety.Entity.DeviceModel,
            DeviceGen = entety.Entity.DeviceGen,
        };
        return CreatedAtAction(nameof(GetPowerDevice), new { id = powerDto.ID }, powerDto);
    }

    // put: "api/Relay/Power/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutPowerDevice(int id, PowerDto updatedPowerDevice)
    {
        if (id != updatedPowerDevice.ID)
        {
            return BadRequest();
        }
        PowerDevice? powerDevice = await _dbContext.PowerDevices.FirstOrDefaultAsync(p => p.ID == id);
        if (powerDevice == null)
        {
            return Problem($"No device with id {id} found.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");
            //return NotFound();
        }

        powerDevice.Name = updatedPowerDevice.Name;
        powerDevice.DeviceType = updatedPowerDevice.DeviceType;
        powerDevice.DeviceName = updatedPowerDevice.DeviceName;
        powerDevice.DeviceId = updatedPowerDevice.DeviceId;
        powerDevice.DeviceMac = updatedPowerDevice.DeviceMac;
        powerDevice.DeviceModel = updatedPowerDevice.DeviceModel;
        powerDevice.DeviceGen = updatedPowerDevice.DeviceGen;

        _dbContext.PowerDevices.Update(powerDevice);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: "api/Relay/Power/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePowerDevice(int id)
    {
        PowerDevice? powerDevice = await _dbContext.PowerDevices.FirstOrDefaultAsync(p => p.ID == id);
        if (powerDevice == null)
        {
            return Problem($"No device with id {id} found.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");
            //return NotFound();
        }

        _dbContext.PowerDevices.Remove(powerDevice);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    // get: "api/Relay/Power/5/switch
    [HttpGet("{id}/switch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeviceChannelState>> GetPowerDeviceSwitch(int id)
    {
        PowerDevice? powerDevice = await _dbContext.PowerDevices.FirstOrDefaultAsync(p => p.ID == id);
        if (powerDevice == null)
        {
            return Problem($"No device with id {id} found.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");
            //return NotFound();
        }

        var deviceService = _strategyDeviceService.Invoke(powerDevice.DeviceType);
        try
        {
            var state = await deviceService.State(powerDevice.HostName, 0);
            return new DeviceChannelState(powerDevice.ID, state);
        }
        catch (DeviceException ex)
        {
            return Problem(ex.InnerException!.Message, ex.Message, StatusCodes.Status400BadRequest, "Bad Request");
            //return BadRequest(ex.Message);
        }
    }

    // put: "api/Relay/Power/5/switch/true|false
    [HttpPut("{id}/switch/{onoff}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutPowerDeviceSwitch(int id, bool onoff)
    {
        PowerDevice? powerDevice = await _dbContext.PowerDevices.FirstOrDefaultAsync(p => p.ID == id);
        if (powerDevice == null)
        {
            return Problem($"No device with id {id} found.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");
            //return NotFound();
        }

        var deviceService = _strategyDeviceService.Invoke(powerDevice.DeviceType);
        try
        {
            await deviceService.Switch(powerDevice.HostName, 0, onoff);
        }
        catch (DeviceException ex)
        {
            return Problem(ex.InnerException!.Message, ex.Message, StatusCodes.Status400BadRequest, "Bad Request");
            //return BadRequest(ex.Message);
        }
        return NoContent();
    }

    // get: "api/Relay/Power/5/webhook
    [HttpGet("{id}/webhook")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<WebHookInfo>>> GetPowerDeviceWebHooks(int id)
    {
        PowerDevice? powerDevice = await _dbContext.PowerDevices.FirstOrDefaultAsync(p => p.ID == id);
        if (powerDevice == null)
        {
            return Problem($"No device with id {id} found.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");
            //return NotFound();
        }

        var deviceService = _strategyDeviceService.Invoke(powerDevice.DeviceType);
        try
        {
            IEnumerable<WebHookInfo>? list = await deviceService.GetHooks(powerDevice.HostName);
            return list.ToList();
        }
        catch (DeviceException ex)
        {
            return Problem(ex.InnerException!.Message, ex.Message, StatusCodes.Status400BadRequest, "Bad Request");
            //return BadRequest(ex.Message);
        }
    }

    // post: "api/Relay/Power/5/webhook/off
    [HttpPost("{id}/webhook/off")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PostPowerDeviceOffWebHook(int id, WebHookInfo webHookInfo)
    {
        PowerDevice? powerDevice = await _dbContext.PowerDevices.FirstOrDefaultAsync(p => p.ID == id);
        if (powerDevice == null)
        {
            return Problem($"No device with id {id} found.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");
            //return NotFound();
        }

        var deviceService = _strategyDeviceService.Invoke(powerDevice.DeviceType);
        try
        {
            await deviceService.CreateTurnOffHook(powerDevice.HostName, 0, webHookInfo.Enabeld, "http://192.168.1.131/api/relay/webhook");
            return NoContent();
        }
        catch (WebHookException ex)
        {
            return Problem(ex.InnerException!.Message, ex.Message, StatusCodes.Status400BadRequest, "Bad Request");
            //return BadRequest(ex.Message);
        }
    }

    // post: "api/Relay/Power/5/webhook/on
    [HttpPost("{id}/webhook/on")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PostPowerDeviceOnWebHook(int id, WebHookInfo webHookInfo)
    {
        PowerDevice? powerDevice = await _dbContext.PowerDevices.FirstOrDefaultAsync(p => p.ID == id);
        if (powerDevice == null)
        {
            return Problem($"No device with id {id} found.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");
            //return NotFound();
        }

        var deviceService = _strategyDeviceService.Invoke(powerDevice.DeviceType);
        try
        {
            await deviceService.CreateTurnOnHook(powerDevice.HostName, 0, webHookInfo.Enabeld, "http://192.168.1.131/api/relay/webhook");
            return NoContent();
        }
        catch (WebHookException ex)
        {
            return Problem(ex.InnerException!.Message, ex.Message, StatusCodes.Status400BadRequest, "Bad Request");
            //return BadRequest(ex.Message);
        }
    }

    // put: "api/Relay/Power/5/webhook
    [HttpPut("{id}/webhook")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutPowerDeviceWebHook(int id, WebHookInfo webHookInfo)
    {
        PowerDevice? powerDevice = await _dbContext.PowerDevices.FirstOrDefaultAsync(p => p.ID == id);
        if (powerDevice == null)
        {
            return Problem($"No device with id {id} found.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");
            //return NotFound();
        }

        var deviceService = _strategyDeviceService.Invoke(powerDevice.DeviceType);
        try
        {
            await deviceService.UpdateHook(powerDevice.HostName, webHookInfo.Id, 0, webHookInfo.Enabeld, webHookInfo.Name, "http://192.168.1.131/api/relay/webhook");
            return NoContent();
        }
        catch (WebHookException ex)
        {
            return Problem(ex.InnerException!.Message, ex.Message, StatusCodes.Status400BadRequest, "Bad Request");
            //return BadRequest(ex.Message);
        }
    }

    // delete: "api/Relay/Power/5/webhook/1
    [HttpDelete("{id}/webhook/{hookid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePowerDeviceWebHook(int id, int hookid)
    {
        PowerDevice? powerDevice = await _dbContext.PowerDevices.FirstOrDefaultAsync(p => p.ID == id);
        if (powerDevice == null)
        {
            return Problem($"No device with id {id} found.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");
            //return NotFound();
        }

        var deviceService = _strategyDeviceService.Invoke(powerDevice.DeviceType);
        try
        {
            await deviceService.DeleteHook(powerDevice.HostName, hookid);
            return NoContent();
        }
        catch (WebHookException ex)
        {
            return Problem(ex.InnerException!.Message, ex.Message, StatusCodes.Status400BadRequest, "Bad Request");
            //return BadRequest(ex.Message);
        }
    }
}
