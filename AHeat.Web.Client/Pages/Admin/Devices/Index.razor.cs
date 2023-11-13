using AHeat.Web.Client.Pages.Admin.Roles;
using System.Data;
using AHeat.Web.Client.Services;
using AHeat.Web.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AHeat.Web.Client.Pages.Admin.Devices;

public partial class Index
{
    [Inject] public IPowerClient powerClient { get; set; } = null!;

    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    public List<PowerDto> PowerDevices { get; set; } = new List<PowerDto>();

    protected override async Task OnInitializedAsync()
    {
        var res = await powerClient.GetPowerDevicesAsync();
        PowerDevices = res.ToList();
    }

    private async Task DiscoverDevice()
    {
        var parameters = new DialogParameters<DiscoverDeviceDialog> { { x => x.Device, new PowerDto() } };

        var dialog = await DialogService.ShowAsync<DiscoverDeviceDialog>("Discover device", parameters);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            if (result.Data is PowerDto)
            {
                var device = ((PowerDto)result.Data);
                var res = await powerClient.PostPowerDeviceAsync(device);
                PowerDevices.Add(res);
                Snackbar.Add($"Device {res.Name} added", Severity.Success);
            }
        }
    }

    private async Task EditDevice(PowerDto device)
    {
        var parameters = new DialogParameters<EditDeviceDialog> { { x => x.device, device } };

        var dialog = await DialogService.ShowAsync<EditDeviceDialog>("Edit Device", parameters);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            if (result.Data is PowerDto)
            {
                device = ((PowerDto)result.Data);
                await powerClient.PutPowerDeviceAsync(device.ID, device);
                Snackbar.Add($"Device {device.Name} updated", Severity.Success);
            }
        }
    }

    private async Task DeleteDevice(PowerDto device)
    {
        bool? result = await DialogService.ShowMessageBox(
            "Warning",
            $"Delete device {device.Name} ?",
            yesText: "Delete!", cancelText: "Cancel");
        if (result != null && result.Value)
        {
            await powerClient.DeletePowerDeviceAsync(device.ID);
            PowerDevices.Remove(device);
            StateHasChanged();
            Snackbar.Add($"Device {device.Name} deleted", Severity.Success);
        }
    }
}
