using AHeat.Web.Client.Services;
using AHeat.Web.Shared.Authorization;
using AHeat.Web.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.AspNetCore.Hosting.Server;

namespace AHeat.Web.Client.Pages.Admin.Roles;

public partial class Index
{
    [Inject]
    public IRolesClient RolesClient { get; set; } = null!;

    [Inject] 
    public IDialogService DialogService { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    public ICollection<RoleDto> Roles { get; set; } = new List<RoleDto>();

    protected override async Task OnInitializedAsync()
    {
        Roles = await RolesClient.GetRolesAsync();
    }

    private async Task AddEditRole(RoleDto role)
    {
        var parameters = new DialogParameters<AddEditRoleDialog> { { x => x.role, role } };

        var dialog = await DialogService.ShowAsync<AddEditRoleDialog>("Add Role", parameters);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            if (result.Data is RoleDto)
            {
                role = ((RoleDto)result.Data);
                if (string.IsNullOrEmpty(role.Id)) 
                {
                    role.Permissions = Permissions.None;
                    role = await RolesClient.PostRoleAsync(role);
                    Roles.Add(role);
                    Snackbar.Add($"Role {role.Name} added", Severity.Success);
                }
                else 
                {
                    await RolesClient.PutRoleAsync(role.Id, role);
                    Snackbar.Add($"Role {role.Name} updated", Severity.Success);
                }
            }
        }
    }

    private async Task DeleteRole(RoleDto role)
    {
        bool? result = await DialogService.ShowMessageBox(
            "Warning",
            $"Delete role {role.Name} ?",
            yesText: "Delete!", cancelText: "Cancel");
        if (result.Value) 
        {
            await RolesClient.DeleteRoleAsync(role.Id);
            Roles.Remove(role);
            StateHasChanged();
            Snackbar.Add($"Role {role.Name} deleted", Severity.Success);
        }
    }
}
