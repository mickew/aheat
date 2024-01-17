using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace AHeat.Web.Client;

public partial class MainLayout : LayoutComponentBase
{
    [Inject]
    protected NavigationManager Navigation { get; set; } = null!;

    protected string Version { get; private set; } = string.Empty;

    protected string AspDotnetVersion { get; private set; } = string.Empty;

    private bool _drawerOpen = false;

    protected override async Task OnInitializedAsync()
    {
        Assembly currentAssembly = typeof(MainLayout).Assembly;
        if (currentAssembly == null)
        {
            currentAssembly = Assembly.GetCallingAssembly();
        }

        AspDotnetVersion = currentAssembly.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName!;
        Version = $"{currentAssembly.GetName().Version!.Major}.{currentAssembly.GetName().Version!.Minor}.{currentAssembly.GetName().Version!.Revision}";
        await base.OnInitializedAsync();
    }

    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    private void BeginLogOut()
    {
        Navigation.NavigateToLogout("authentication/logout", "/");
    }
}
