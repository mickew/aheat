using AHeat.Application.Models.Shelly;
using AHeat.Web.Shared;

namespace AHeat.Application.Interfaces;
public interface IDeviceService
{
    Task Switch(string url, int channel, bool onOff);
    Task<IEnumerable<WebHookInfo>> GetHooks(string url);
    Task CreateTurnOnHook(string url, int channel, bool enabeld, string hookEndpoint);
    Task CreateTurnOffHook(string url, int channel, bool enabeld, string hookEndpoint);
    Task UpdateHook(string url, int id, int channel, bool enabeld);
    Task DeleteHook(string url, int id);
}
