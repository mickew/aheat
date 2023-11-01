using AHeat.Application.Exceptions;
using AHeat.Application.Interfaces;
using AHeat.Web.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AHeat.Web.API.Controllers.Relay;

[ApiController]
[Route("api/relay/[controller]")]
public class DiscoverController : ControllerBase
{
    private readonly IDiscoverService _discoverService;

    public DiscoverController(IDiscoverService discoverService)
    {
        _discoverService = discoverService;
    }

    // Get: "api/Relay/Discover
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DicoverInfo>> Discover(string url)
    {
        try
        {
            DicoverInfo info = await _discoverService.Discover(url);
            if (info == null)
            {
                return NotFound("null returned");
            }
            return info;
        }
        catch (DiscoverException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
