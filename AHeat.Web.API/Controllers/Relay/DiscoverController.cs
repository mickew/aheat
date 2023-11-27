using AHeat.Application.Exceptions;
using AHeat.Application.Interfaces;
using AHeat.Application.Services;
using AHeat.Web.Shared;
using AHeat.Web.Shared.Authorization;
using AHeat.Web.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace AHeat.Web.API.Controllers.Relay;

[ApiController]
[Route("api/relay/[controller]")]
public class DiscoverController : ControllerBase
{
    private readonly ILogger<DiscoverController> _logger;
    private readonly StrategyDiscoverService _strategyDiscoverService;

    public DiscoverController(ILogger<DiscoverController> logger, StrategyDiscoverService strategyDiscoverService)
    {
        _logger = logger;
        _strategyDiscoverService = strategyDiscoverService;
    }

    // Get: "api/Relay/Discover
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Permissions.ConfigurePowerDevices)]
    public async Task<ActionResult<DicoverInfo>> Discover(string url)
    {
        try
        {
            DicoverInfo info = null!;
            var length = Enum.GetNames(typeof(DeviceTypes)).Length;
            for (int i = 1; i < length; i++)
            {
                try
                {
                    var service = _strategyDiscoverService.Invoke((DeviceTypes)i);
                    info = await service.Discover(url);
                    if (info != null)
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                }
            }

            if (info == null)
            {
                _logger.LogError("null returned when discovering device at url {url}", url);
                return NotFound($"null returned when discovering device at url {url}");
            }
            return info;
        }
        catch (DiscoverException ex)
        {
            _logger.LogError(ex, "Error when discovering device at url {url}", url);
            return NotFound($"Error when discovering device at url {url}");
        }
    }
}
