using System.Net.Http;
using System.Numerics;
using AHeat.Application.Exceptions;
using AHeat.Application.Interfaces;
using AHeat.Application.Models.Shelly;
using AHeat.Web.Shared;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AHeat.Application.Services;
public class DiscoverShelly2Service : IDiscoverService
{
    private readonly ILogger<DiscoverShelly2Service> _logger;
    private readonly IHttpClientFactory _clientFactory;

    public DiscoverShelly2Service(ILogger<DiscoverShelly2Service> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public async Task<DicoverInfo> Discover(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{url}/rpc/Shelly.GetDeviceInfo");
        var client = _clientFactory.CreateClient();
        try
        {
            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var apiString = await response.Content.ReadAsStringAsync();
                var deviceInfo = JsonConvert.DeserializeObject<DeviceInfo>(apiString);
                DicoverInfo dicoverInfo = new(url, deviceInfo!.Name, deviceInfo.Id, deviceInfo.Mac, deviceInfo.Model, deviceInfo.Gen);
                return dicoverInfo;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new DiscoverException($"Error when discovering uir {url}", ex);
        }        
        return null!;
    }
}
