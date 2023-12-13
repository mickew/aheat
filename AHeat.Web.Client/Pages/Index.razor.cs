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

    public List<ClimatDevice> ClimatDevices { get; set; } = new List<ClimatDevice>();

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
        _hubConnection.On<string, string, string, string>("ClimateUpdate", (id, name, temperature, humidity) =>
        {
            Console.WriteLine("ClimateUpdate received");
            var climateToUpdate = ClimatDevices.FirstOrDefault(d => d.Id == id);
            if (climateToUpdate == null)
            {
                ClimatDevices.Add(new ClimatDevice() { Id = id, Name = name, Temperature = temperature, Humidity = humidity });
            }
            else
            {
                climateToUpdate.Temperature = temperature;
                climateToUpdate.Humidity = humidity;
            }
            StateHasChanged();
        });
        await _hubConnection.StartAsync();
        Console.WriteLine("_hubConnection started");
    }

    private async Task ToggleDevice(int id, bool state)
    {
        var device = Devices.Where(x => x.Id == id).FirstOrDefault();
        device!.Prosessing = true;
        StateHasChanged();
        await powerClient.PutPowerDeviceSwitchAsync(id, !state);
        await Task.Delay(1000);
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

    public class ClimatDevice
    {
        public string? Id { get; init; }
        public string? Name { get; init; }
        public string? Temperature { get; set; }
        public string? Humidity { get; set; }
    }
}
