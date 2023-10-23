using AHeat.Web.Shared.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AHeat.Web.API.Models;

public class Role : IdentityRole
{
    public Permissions Permissions { get; set; }
}
