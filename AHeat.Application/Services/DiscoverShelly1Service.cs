using AHeat.Application.Exceptions;
using AHeat.Application.Interfaces;
using AHeat.Application.Models.Shelly;
using AHeat.Web.Shared;
using AHeat.Web.Shared.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AHeat.Application.Services;
public class DiscoverShelly1Service : IDiscoverService
{
    private readonly ILogger<DiscoverShelly1Service> _logger;
    private readonly IHttpClientFactory _clientFactory;

    public DiscoverShelly1Service(ILogger<DiscoverShelly1Service> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public async Task<DicoverInfo> Discover(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{url}/settings");
        var client = _clientFactory.CreateClient();
        try
        {
            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var apiString = await response.Content.ReadAsStringAsync();
                var deviceInfoSettings = JsonConvert.DeserializeObject<DeviceInfoSettings>(apiString);
                DicoverInfo dicoverInfo = new(url, DeviceTypes.ShellyGen1, deviceInfoSettings!.Name, deviceInfoSettings.Device.Hostname, deviceInfoSettings.Device.Mac, deviceInfoSettings.Device.Type, 1);
                return dicoverInfo;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new DiscoverException($"Error when discovering url at {url}", ex);
        }
        return null!;
    }
}
