using AHeat.Web.Client.Services;
using AHeat.Web.Shared.Authorization;
using AHeat.Web.Shared;
using Microsoft.AspNetCore.Components;

namespace AHeat.Web.Client.Pages.Admin.AccessControl;

public partial class Index
{
    [Inject]
    private IAccessControlClient AccessControlClient { get; set; } = null!;

    private AccessControlVm? _vm;

    protected override async Task OnInitializedAsync()
    {
        _vm = await AccessControlClient.GetConfigurationAsync();
    }

    private async Task Set(RoleDto role, Permissions permission, bool granted)
    {
        role.Set(permission, granted);

        await AccessControlClient.UpdateConfigurationAsync(role);
    }
}
