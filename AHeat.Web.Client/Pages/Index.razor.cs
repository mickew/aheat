using AHeat.Web.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace AHeat.Web.Client.Pages;

public partial class Index : IAsyncDisposable
{
    [Inject] public IPowerClient powerClient { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public NavigationManager? Navigation { get; set; } = null!;

    public List<Device> Devices { get; set; } = new List<Device>();

    private HubConnection? _hubConnection;

    protected override async Task OnInitializedAsync()
    {
        _hubConnection = new HubConnectionBuilder().WithUrl(Navigation!.ToAbsoluteUri("/statushub")).Build();
        var powerDevices = await powerClient.GetPowerDevicesAsync();
        Devices.Clear();
        foreach (var device in powerDevices)
        {
            Devices.Add(new Device() { Id = device.ID, Mac = device.DeviceMac, Name = device.Name });
        }
        foreach (var device in Devices)
        {
            var status = await powerClient.GetPowerDeviceSwitchAsync(device.Id);
            device.State = status.state;
        }
        _hubConnection.On<string, string>("StatusUpdate", (mac, onoff) =>
        {
            Console.WriteLine("StatusUpdate received");
            var deviceToUpdate = Devices.FirstOrDefault(d => d.Mac == mac);
            if (deviceToUpdate != null)
            {
                if (onoff == "true")
                {
                    deviceToUpdate.State = true;
                }
                else
                {
                    deviceToUpdate.State = false;
                }
                StateHasChanged();
            }
        });
        await _hubConnection.StartAsync();
        Console.WriteLine("_hubConnection started");
    }

    private async Task ToggleDevice(int id, bool state)
    {
        var device = Devices.Where(x => x.Id == id).FirstOrDefault();
        device!.Prosessing = true;
        await powerClient.PutPowerDeviceSwitchAsync(id, !state);
        await Task.Delay(500);
        //var status = await powerClient.GetPowerDeviceSwitchAsync(id);
        //device.State = status.state;
        device!.Prosessing = false;
        var onoff = device.State ? "turned on" : "turned off";
        if (state != !device.State)
        {
            onoff = "toggled";
        }
        Snackbar.Add($"{device.Name} {onoff}!", Severity.Success);
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }

    public class Device
    {
        public int Id { get; init; }
        public string? Mac { get; init; }
        public string? Name { get; init; }
        public bool State { get; set; }
        public bool Prosessing { get; set; }
    }
}
