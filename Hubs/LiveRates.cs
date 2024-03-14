using Microsoft.AspNetCore.SignalR;
using SignalRLiveRates.Service;

namespace SignalRLiveRates.Hubs
{
    public class LiveRatesHub : Hub
    {
        private readonly IHubContext<LiveRatesHub> _liveRatesHub;
        public LiveRatesHub(IHubContext<LiveRatesHub> liveRatesHub)
        {
            _liveRatesHub = liveRatesHub;
        }
        public async Task GetRates()
        {
            var rates = await new LiveRatesGenerator(_liveRatesHub).GetRates();
            await Clients.Caller.SendAsync("ReceiveRates", rates);
            //await Clients.Caller.SendAsync("ReceiveRates", rates);
        }
        public async Task SendRates(string rates)
        {
            await Clients.All.SendAsync("ReceiveRates", rates);
        }

    }
}