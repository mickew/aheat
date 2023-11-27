using AHeat.Web.Client.Services;
using AHeat.Web.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AHeat.Web.Client.Pages.Admin.Users;

public partial class Index
{
    [Inject] public IUsersClient UsersClient { get; set; } = null!;

    [Inject] public IRolesClient RolesClient { get; set; } = null!;

    [Inject] public NavigationManager? Navigation { get; set; }

    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public ILogger<Index> Logger { get; set; } = null!;

    protected bool UserGotNoRights { get; set; } = true;

    public ICollection<UserDto> Users { get; set; } = new List<UserDto>();

    protected override async Task OnInitializedAsync()
    {
        Users = await UsersClient.GetUsersAsync();
    }

    private async Task AddEditUser(UserDto user, bool add)
    {
        var roles = await RolesClient.GetRolesAsync();
        UserDto theUser;
        if (add) 
        {
            theUser = user;
        }
        else
        {
            theUser = await UsersClient.GetUserAsync(user.Id);
        }
        var parameters = new DialogParameters();
        parameters.Add(nameof(AddEditUserDialog.User), theUser);
        parameters.Add(nameof(AddEditUserDialog.Roles), roles);
        //var parameters = new DialogParameters<AddEditUserDialog> { { x => x.User, user } };

        var dialog = await DialogService.ShowAsync<AddEditUserDialog>(add ? "Add User!" : "Edit User!", parameters);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            if (result.Data is UserDto)
            {
                theUser = ((UserDto)result.Data);
                if (string.IsNullOrEmpty(user.Id))
                {
                    try
                    {
                        theUser = await UsersClient.PostUserAsync(theUser);
                    }
                    catch (ApiException ex)
                    {
                        Logger.LogError(ex, "Error adding user");
                        Logger.LogError(ex.Message);
                        Snackbar.Add(ex.Message, Severity.Error);
                        return;
                    }
                    Users.Add(theUser);
                    Snackbar.Add($"User {user.UserName} added", Severity.Success);
                }
                else
                {
                    try
                    {
                        await UsersClient.PutUserAsync(theUser.Id, theUser);
                    }
                    catch (ApiException ex)
                    {
                        Logger.LogError(ex, "Error updating user");
                        Logger.LogError(ex.Message);
                        Snackbar.Add(ex.Message, Severity.Error);
                        return;
                    }
                    Users.Remove(user);
                    Users.Add(theUser);
                    Snackbar.Add($"User {user.UserName} updated", Severity.Success);
                }
                StateHasChanged();
            }
        }
    }

    private async Task DeleteUser(UserDto user)
    {
        bool? result = await DialogService.ShowMessageBox(
            "Warning",
            $"Delete user {user.UserName} ?",
            yesText: "Delete!", cancelText: "Cancel");
        if (result != null && result.Value)
        {
            try
            {
                await UsersClient.DeleteUserAsync(user.Id);
            }
            catch (ApiException ex)
            {
                Logger.LogError(ex, "Error deleting user");
                Logger.LogError(ex.Message);
                Snackbar.Add(ex.Message, Severity.Error);
                return;
            }
            Users.Remove(user);
            StateHasChanged();
            Snackbar.Add($"User {user.UserName} deleted", Severity.Success);
        }
    }

    private async Task ResetPassword(UserDto user)
    {
        bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                                  $"Reset password for user {user.UserName} ?",
                                             yesText: "Reset!", cancelText: "Cancel");
        if (result != null && result.Value)
        {
            try
            {
                await UsersClient.ResetPasswordAsync(user.Id);
            }
            catch (ApiException ex)
            {
                Logger.LogError(ex, "Error resetting password");
                Logger.LogError(ex.Message);
                Snackbar.Add(ex.Message, Severity.Error);
                return;
            }
            Snackbar.Add($"Password for user {user.UserName} reset", Severity.Success);
        }
    }
}
