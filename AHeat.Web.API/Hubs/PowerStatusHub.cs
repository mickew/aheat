using Microsoft.AspNetCore.SignalR;

namespace AHeat.Web.API.Hubs;

public class PowerStatusHub : Hub
{
    private readonly ILogger<PowerStatusHub> _logger;

    public PowerStatusHub(ILogger<PowerStatusHub> logger)
    {
        _logger = logger;
        
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
