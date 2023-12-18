using AHeat.Web.Client.Services;
using AHeat.Web.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AHeat.Web.Client.Pages.Admin.Climates;

public partial class Index
{
    [Inject] public IClimateClient climateClient { get; set; } = null!;

    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    protected bool UserGotNoRights { get; set; } = true;

    public List<ClimateDeviceDto> ClimateDevices { get; set; } = new List<ClimateDeviceDto>();

    protected override async Task OnInitializedAsync()
    {
        var res = await climateClient.GetClimateDevicesAsync();
        ClimateDevices = res.ToList();
    }

    private async Task EditDevice(ClimateDeviceDto climateDevice)
    {
        var parameters = new DialogParameters<EditClimateDeviceDialog> { { x => x.climateDevice, climateDevice } };
        var dialog = await DialogService.ShowAsync<EditClimateDeviceDialog>("Edit Climate device", parameters);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            if (result.Data is ClimateDeviceDto)
            {
                climateDevice = ((ClimateDeviceDto)result.Data);
                await climateClient.UpdateClimateDeviceAsync(climateDevice);
                Snackbar.Add($"climate device {climateDevice.DeviceId} updated", Severity.Success);
            }
        }
    }

    private async Task DeleteDevice(ClimateDeviceDto climateDevice)
    {
        bool? result = await DialogService.ShowMessageBox(
            "Warning",
            $"Delete climate device {climateDevice.DeviceId} ?",
            yesText: "Delete!", cancelText: "Cancel");
        if (result != null && result.Value)
        {
            await climateClient.DeleteClimateDeviceAsync(climateDevice.DeviceId);
            ClimateDevices.Remove(climateDevice);
            StateHasChanged();
            Snackbar.Add($"Climate device {climateDevice.DeviceId} deleted", Severity.Success);
        }
    }
}

