using AHeat.Web.Client;
using AHeat.Web.Client.Authorization;
using AHeat.Web.Client.Services;
using AHeat.Web.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("AHeat.Web.API", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("AHeat.Web.API"));

builder.Services
    .AddApiAuthorization()
    .AddAccountClaimsPrincipalFactory<CustomAccountClaimsPrincipalFactory>();

builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, FlexibleAuthorizationPolicyProvider>();

builder.Services.Scan(scan => scan
    .FromAssemblyOf<IAccessControlClient>()
    .AddClasses(classes =>
        classes.InExactNamespaceOf<IAccessControlClient>())
    .AsImplementedInterfaces()
    .WithScopedLifetime());

await builder.Build().RunAsync();
