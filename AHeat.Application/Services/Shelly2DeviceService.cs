using System.Text;
using AHeat.Application.Exceptions;
using AHeat.Application.Interfaces;
using AHeat.Application.Models.Shelly;
using AHeat.Web.Shared;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AHeat.Application.Services;
public class Shelly2DeviceService : IDeviceService
{
    private readonly ILogger<Shelly2DeviceService> _logger;
    private readonly IHttpClientFactory _clientFactory;

    public Shelly2DeviceService(ILogger<Shelly2DeviceService> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public async Task CreateTurnOffHook(string url, int channel, bool enabeld, string hookEndpoint)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{url}/rpc");
        var urlList = new List<string> { $"{hookEndpoint}?mac=${{config.sys.device.mac}}&switch=${{status[\"switch:0\"].output}}" };
        CreateWebHook createWebHook = new CreateWebHook(1, "Webhook.Create", new CreateWebHookParams(channel, enabeld, "switch.off", "OffHookSender", urlList));
        var s = JsonConvert.SerializeObject(createWebHook, Formatting.None, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        var content = new StringContent(s, Encoding.UTF8, "application/json");
        var client = _clientFactory.CreateClient();
        try
        {
            HttpResponseMessage response = await client.PostAsync($"{url}/rpc", content);
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
                throw new WebHookException(response.Content.ToString()!);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new WebHookException($"Error when creating webhook at {url}", ex);
        }
    }

    public async Task CreateTurnOnHook(string url, int channel, bool enabeld, string hookEndpoint)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{url}/rpc");
        var urlList = new List<string> { $"{hookEndpoint}?mac=${{config.sys.device.mac}}&switch=${{status[\"switch:0\"].output}}" };
        CreateWebHook createWebHook = new CreateWebHook(1, "Webhook.Create", new CreateWebHookParams(channel, enabeld, "switch.on", "OnHookSender", urlList));
        var s = JsonConvert.SerializeObject(createWebHook, Formatting.None, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        var content = new StringContent(s, Encoding.UTF8, "application/json");
        var client = _clientFactory.CreateClient();
        try
        {
            HttpResponseMessage response = await client.PostAsync($"{url}/rpc", content);
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
                throw new WebHookException(response.Content.ToString()!);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new WebHookException($"Error when creating webhook at {url}", ex);
        }
    }

    public async Task DeleteHook(string url, int id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{url}/rpc/Webhook.Delete?id={id}");
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
                throw new WebHookException(response.Content.ToString()!);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new WebHookException($"Error when deleting webhook at {url}", ex);
        }
    }

    public async Task<IEnumerable<WebHookInfo>> GetHooks(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{url}/rpc/Webhook.List");
        var client = _clientFactory.CreateClient();
        try
        {
            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var apiString = await response.Content.ReadAsStringAsync();
                WebHooks? hooks = JsonConvert.DeserializeObject<WebHooks>(apiString);
                List<WebHookInfo> list = new List<WebHookInfo>();
                foreach (var item in hooks!.Hooks)
                {
                    list.Add(new WebHookInfo(item.Id, item.Name, item.Enable));
                }
                return list;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new WebHookException($"Error when getting webhooks at {url}", ex);
        }
        return null!;
    }

    public async Task<bool> State(string url, int channel)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{url}/rpc/Switch.GetStatus?id={channel}");
        var client = _clientFactory.CreateClient();
        try
        {
            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var apiString = await response.Content.ReadAsStringAsync();
                ChannelStatus? result = JsonConvert.DeserializeObject<ChannelStatus>(apiString);
                return result!.Output;
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

    public async Task Switch(string url, int channel, bool onOff)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{url}/rpc/Switch.Set?id={channel}&on={onOff.ToString().ToLower()}");
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

    public async Task UpdateHook(string url, int id, int channel, bool enabeld, string name, string hookEndpoint)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{url}/rpc");
        var urlList = new List<string> { $"{hookEndpoint}?mac=${{config.sys.device.mac}}&switch=${{status[\"switch:0\"].output}}" };
        UpdateWebHook updateWebHook = new UpdateWebHook(1, "Webhook.Update", new UpdateWebHookParams(id, channel, enabeld, name, urlList));
        var s = JsonConvert.SerializeObject(updateWebHook, Formatting.None, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        var content = new StringContent(s, Encoding.UTF8, "application/json");
        var client = _clientFactory.CreateClient();
        try
        {
            HttpResponseMessage response = await client.PostAsync($"{url}/rpc", content);
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
                throw new WebHookException(response.Content.ToString()!);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new WebHookException($"Error when updating webhook at {url}", ex);
        }
    }
}
