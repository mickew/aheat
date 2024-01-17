using AHeat.Web.Shared.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Security.Claims;

namespace AHeat.Web.Client.Shared;

public partial class LoginDisplay
{
    [Inject]
    public NavigationManager? Navigation { get; set; } = null!;

    private void BeginLogOut()
    {
        Navigation.NavigateToLogout("authentication/logout", "/");
    }

    private string GetFullName(ClaimsPrincipal principal)
    {
        var firstName = principal.Claims.FirstOrDefault(
            c => c.Type == CustomClaimTypes.FirstName);
        var lastName = principal.Claims.FirstOrDefault(
            c => c.Type == CustomClaimTypes.LastName);
        if (firstName != null && lastName != null)
        {
            return $"{firstName.Value} {lastName.Value}";
        }
        return "?";
    }
}
