using AHeat.Web.Client.Pages;
using AHeat.Web.Client.Services;
using AHeat.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using MudBlazor;

namespace AHeat.Web.Client.Shared;

public partial class NavMenu
{
    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    [Inject]
    public IUsersClient UsersClient { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;


    private async Task ChangePassword()
    {
        var parameters = new DialogParameters<ChangePasswordDialog> { { x => x.ChangePassword, new ChangePasswordDto() } };
        var dialog = await DialogService.ShowAsync<ChangePasswordDialog>("Change Password", parameters);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            if (result.Data is ChangePasswordDto)
            {
                var changePassword = ((ChangePasswordDto)result.Data);
                try
                {
                    await UsersClient.ChangePasswordAsync(changePassword);
                    Snackbar.Add($"Password changed", Severity.Success);
                }
                catch (ApiException<ProblemDetails> ex)
                {
                    Snackbar.Add($"Error when changing password: {ex.Result.Title} {ex.Result.Detail}", Severity.Error);
                }
            }
        }
    }
}
