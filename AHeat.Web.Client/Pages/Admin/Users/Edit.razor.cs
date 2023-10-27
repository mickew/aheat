using System.Data;
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
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public NavigationManager Navigation { get; set; } = null!;

    public MudForm? form;

    public UserDtoFluentValidator UserDtoValidator = new UserDtoFluentValidator();

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
        await form!.Validate();
        if (form!.IsValid)
        {
            await UsersClient.PutUserAsync(User.Id, User);
            Snackbar.Add($"Role {User.UserName} updated", Severity.Success);

            Navigation.NavigateTo("/admin/users");
        }
    }
}
