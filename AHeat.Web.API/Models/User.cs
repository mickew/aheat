using Microsoft.AspNetCore.Identity;

namespace AHeat.Web.API.Models;

public class User : IdentityUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}
