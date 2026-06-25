using Microsoft.AspNetCore.SignalR;

namespace sistemaapuestas.Hubs;

public class MatchHub : Hub
{
    public async Task NotifyMatchUpdate()
    {
        await Clients.All.SendAsync("MatchesUpdated");
    }
}
