using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;

using Microsoft.AspNetCore.SignalR;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;

namespace SignalRLiveRates.Service
{
    public class LiveRatesService
    {
        const string LIVE_RATES_KEY = "39e5f1e8ac";
        const string CURRENCY_LOOKUP = "EURUSD,EURGBP";//"EUR_USD,EUR_GBP";
                                                       //HubConnection connectionServer;

        HubConnection connectionClient;
        // HubConnection connection;
        public LiveRatesService()
        {
            //connectionServer = new HubConnectionBuilder().WithUrl($"https://wss.live-rates.com/api/price?rate={CURRENCY_LOOKUP}&key={LIVE_RATES_KEY}", options =>
            // connectionServer = new HubConnectionBuilder().WithUrl($"https://wss3.live-rates.com", options =>
            // {
            //     options.Transports = HttpTransportType.WebSockets;
            // }).WithAutomaticReconnect().Build();

            // connectionServer.Closed += async (error) =>
            // {
            //     await Task.Delay(new Random().Next(0, 5) * 1000);
            //     await connectionServer.StartAsync();
            // };

        }
        public async Task InitLiveRates()
        {
            var connectionServer = new SocketIO("https://wss.live-rates.com");
            connectionServer.OnConnected += async (sender, e) =>
            {

                // USE THIS LIST TO FILTER AND RECEIVE ONLY INSTRUMENTS YOU NEED. LEAVE EMPTY TO RECEIVE ALL
                // If you want to subscribe only specific instruments, emit instruments. To receive all instruments, comment the line below.
                await connectionServer.EmitAsync("instruments", CURRENCY_LOOKUP);

                // Use the 'trial' as key to establish a 2-minute streaming connection with real-time data.
                // After the 2-minute test, the server will drop the connection and block the IP for an Hour.
                await connectionServer.EmitAsync("key", LIVE_RATES_KEY);

            };
            connectionServer.On("rates", async (response) =>
            {
                string text = response.GetValue<string>();

                connectionClient = new HubConnectionBuilder().WithUrl("https://localhost:7040/liveRatesHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                }).WithAutomaticReconnect().Build();

                connectionClient.Closed += async (error) =>
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await connectionClient.StartAsync();
                };

                try
                {
                    await connectionClient.StartAsync();
                    await connectionClient.InvokeAsync("SendRates", response);
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
            try
            {
                await connectionServer.ConnectAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // connectionServer = new HubConnectionBuilder().WithUrl($"https://wss.live-rates.com", options =>
            // {
            //     options.Transports = HttpTransportType.WebSockets;
            // }).WithAutomaticReconnect().Build();

            // connectionServer.Closed += async (error) =>
            // {
            //     await Task.Delay(new Random().Next(0, 5) * 1000);
            //     await connectionServer.StartAsync();
            // };
            // Emit events after connection established
            // connectionServer.On("Connected", async () =>
            // {
            //     // USE THIS LIST TO FILTER AND RECEIVE ONLY INSTRUMENTS YOU NEED. LEAVE EMPTY TO RECEIVE ALL
            //     // If you want to subscribe only specific instruments, emit instruments. To receive all instruments, comment the line below.
            //     await connectionServer.InvokeAsync("instruments", CURRENCY_LOOKUP);

            //     // Use the 'trial' as key to establish a 2-minute streaming connection with real-time data.
            //     // After the 2-minute test, the server will drop the connection and block the IP for an Hour.
            //     await connectionServer.InvokeAsync("key", LIVE_RATES_KEY);
            // });

            // connectionServer.On<string>("rates", async (response) =>
            // {

            //     Console.WriteLine(response);

            //     connectionClient = new HubConnectionBuilder().WithUrl("/liveRatesHub", options =>
            //     {
            //         options.Transports = HttpTransportType.WebSockets;
            //     }).WithAutomaticReconnect().Build();

            //     connectionClient.Closed += async (error) =>
            //     {
            //         await Task.Delay(new Random().Next(0, 5) * 1000);
            //         await connectionClient.StartAsync();
            //     };

            //     try
            //     {
            //         await connectionClient.InvokeAsync("SendRates", response);
            //     }

            //     catch (Exception ex)
            //     {
            //         Console.WriteLine(ex.Message);
            //     }

            // });
            // try
            // {
            //     await connectionServer.StartAsync();
            // }
            // catch (Exception ex)
            // {
            //     Console.WriteLine(ex.Message);
            // }
        }
    }
}