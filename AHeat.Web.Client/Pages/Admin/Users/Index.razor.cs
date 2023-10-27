using AHeat.Web.Client.Services;
using AHeat.Web.Shared;
using Microsoft.AspNetCore.Components;

namespace AHeat.Web.Client.Pages.Admin.Users;

public partial class Index
{
    [Inject] public IUsersClient UsersClient { get; set; } = null!;

    [Inject] public NavigationManager? Navigation { get; set; }

    protected bool UserGotNoRights { get; set; } = true;

    public ICollection<UserDto> Users { get; set; } = new List<UserDto>();

    protected override async Task OnInitializedAsync()
    {
        Users = await UsersClient.GetUsersAsync();
    }

    protected void EditUser(string id)
    {
        Navigation!.NavigateTo($"/admin/users/{id}");
    }
}
