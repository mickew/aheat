using AHeat.Web.Client.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AHeat.Web.Client.Pages;

public partial class Index
{
    [Inject] public IPowerClient powerClient { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    public List<Device> Devices { get; set; } = new List<Device>();

    protected override async Task OnInitializedAsync()
    {
        var powerDevices = await powerClient.GetPowerDevicesAsync();
        Devices.Clear();
        foreach (var device in powerDevices)
        {
            Devices.Add(new Device() { Id = device.ID, Name = device.Name });
        }
        foreach (var device in Devices)
        {
            var status = await powerClient.GetPowerDeviceSwitchAsync(device.Id);
            device.State = status.state;
        }
    }

    private async Task ToggleDevice(int id, bool state)
    {
        var device = Devices.Where(x => x.Id == id).FirstOrDefault();
        device!.Prosessing = true;
        await powerClient.PutPowerDeviceSwitchAsync(id, !state);
        await Task.Delay(500);
        var status = await powerClient.GetPowerDeviceSwitchAsync(id);
        device.State = status.state;
        device!.Prosessing = false;
        var onoff = device.State ? "turned on" : "turned off";
        Snackbar.Add($"{device.Name} {onoff}!", Severity.Success);
    }

    public class Device
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public bool State { get; set; }
        public bool Prosessing { get; set; }
    }
}
