using Microsoft.AspNetCore.SignalR;

namespace SignalRLiveRates.Hubs
{
    public class LiveRatesServerHub : Hub
    {
        public async Task rates(string rates)
        {
            await Clients.All.SendAsync("rates", rates);
        }
    }
}