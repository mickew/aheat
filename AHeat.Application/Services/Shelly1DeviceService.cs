using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHeat.Application.Exceptions;
using AHeat.Application.Interfaces;
using AHeat.Application.Models.Shelly;
using AHeat.Web.Shared;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AHeat.Application.Services;
public class Shelly1DeviceService : IDeviceService
{
    private readonly ILogger<Shelly1DeviceService> _logger;
    private readonly IHttpClientFactory _clientFactory;

    public Shelly1DeviceService(ILogger<Shelly1DeviceService> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public Task CreateTurnOffHook(string url, int channel, bool enabeld, string hookEndpoint)
    {
        throw new NotImplementedException();
    }

    public Task CreateTurnOnHook(string url, int channel, bool enabeld, string hookEndpoint)
    {
        throw new NotImplementedException();
    }

    public Task DeleteHook(string url, int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<WebHookInfo>> GetHooks(string url)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> State(string url, int channel)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{url}/relay/{channel}");
        var client = _clientFactory.CreateClient();
        try
        {
            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var apiString = await response.Content.ReadAsStringAsync();
                RelayStatus? result = JsonConvert.DeserializeObject<RelayStatus>(apiString);
                return result!.Ison;
            }
            else
            {
                _logger.LogError(response.Content.ToString());
                throw new DeviceException(response.Content.ToString()!);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new DeviceException($"Error when getting device status at {url}", ex);
        }
    }

    private string ReturnOnOff(bool onOff)
    {
        return onOff ? "on" : "off";
    }

    public async Task Switch(string url, int channel, bool onOff)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{url}/relay/{channel}?turn={ReturnOnOff(onOff)}");
        var client = _clientFactory.CreateClient();
        try
        {
            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var apiString = await response.Content.ReadAsStringAsync();
                ReturnResultError? result = JsonConvert.DeserializeObject<ReturnResultError>(apiString);
                if (result!.Error != null)
                {
                    _logger.LogError(result.Error.Message);
                    throw new WebHookException(result.Error.Message);
                }
            }
            else
            {
                _logger.LogError(response.Content.ToString());
                throw new DeviceException(response.Content.ToString()!);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new DeviceException($"Error when switching device at {url}", ex);
        }
    }

    public Task UpdateHook(string url, int id, int channel, bool enabeld, string name, string hookEndpoint)
    {
        throw new NotImplementedException();
    }
}
