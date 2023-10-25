using AHeat.Web.Client.Services;
using AHeat.Web.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AHeat.Web.Client.Pages.Admin.Users;

public partial class Edit
{
    [Parameter]
    public string UserId { get; set; } = null!;

    [Inject]
    public IUsersClient UsersClient { get; set; } = null!;

    [Inject]
    public IRolesClient RolesClient { get; set; } = null!;

    [Inject]
    public NavigationManager Navigation { get; set; } = null!;

    public bool success;
    public string[] errors = { };
    public MudForm? form;

    public UserDto User { get; set; } = new();

    public ICollection<RoleDto> Roles { get; set; } = new List<RoleDto>();

    protected override async Task OnParametersSetAsync()
    {
        Roles = await RolesClient.GetRolesAsync();

        User = await UsersClient.GetUserAsync(UserId);
    }

    public void ToggleSelectedRole(string roleName)
    {
        if (User.Roles.Contains(roleName))
        {
            User.Roles.Remove(roleName);
        }
        else
        {
            User.Roles.Add(roleName);
        }

        StateHasChanged();
    }

    public async Task UpdateUser()
    {
        await UsersClient.PutUserAsync(User.Id, User);

        Navigation.NavigateTo("/admin/users");
    }
}
