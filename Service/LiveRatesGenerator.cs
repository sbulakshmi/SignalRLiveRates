using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using SocketIOClient;
using SignalRLiveRates.Hubs;
using SocketIOClient.Newtonsoft;
using Microsoft.AspNetCore.Http.Connections;
using System.Threading;
using System.Net.Http;

namespace SignalRLiveRates.Service
{
    public class LiveRatesGenerator
    {
        #region Constants
        const string LIVE_RATES_KEY =  "trial";//
        const string CURRENCY_LOOKUP = "EURUSD,EURGBP";//"EUR_USD,EUR_GBP";
                                                       //HubConnection connectionServer;
                                                       
        const string STREAM_BASE_URL = "https://wss3.live-rates.com";
        // Base URL of the Web API

        const string  BASE_URL = "https://live-rates.com/";

        // Endpoint to call
         const string END_POINT = $"api/price?rate={CURRENCY_LOOKUP}&key={LIVE_RATES_KEY}";   
         #endregion                                                    

        private readonly IHubContext<LiveRatesHub> _liveRatesHub;
        private Timer _timer;
        HubConnection connectionClient;
        public LiveRatesGenerator(IHubContext<LiveRatesHub> liveRatesHub)
        {
            _liveRatesHub = liveRatesHub;
        }


        private async void UpdateExchangePrice(object state)
        {
            string text = new Random().Next(101, 113).ToString();
            Console.WriteLine(text);
            try
            {
                // connectionClient = new HubConnectionBuilder().WithUrl("https://localhost:7040/liveRatesHub", options =>
                // {
                //     options.Transports = HttpTransportType.WebSockets;
                // }).WithAutomaticReconnect().Build();

                // connectionClient.Closed += async (error) =>
                // {
                //     await Task.Delay(new Random().Next(0, 5) * 1000);
                //     await connectionClient.StartAsync();
                // };
                await _liveRatesHub.Clients.All.SendAsync("ReceiveRates", text);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        // public async Task InitLiveRates()
        // {
        //     _timer = new Timer(UpdateExchangePrice, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        // }

        public async Task InitLiveRates()
        {
            var connectionServer = new SocketIO($"{STREAM_BASE_URL}");
            connectionServer.OnConnected += async (sender, e) =>
            {

                // USE THIS LIST TO FILTER AND RECEIVE ONLY INSTRUMENTS YOU NEED. LEAVE EMPTY TO RECEIVE ALL
                // If you want to subscribe only specific instruments, emit instruments. To receive all instruments, comment the line below.
                await connectionServer.EmitAsync("instruments", CURRENCY_LOOKUP);

                // Use the 'trial' as key to establish a 2-minute streaming connection with real-time data.
                // After the 2-minute test, the server will drop the connection and block the IP for an Hour.
                await connectionServer.EmitAsync("key", LIVE_RATES_KEY);

            };
            connectionServer.On("api/price", response =>
            {
                // You can print the returned data first to decide what to do next.
                // output: ["hi client"]
                Console.WriteLine(response);

                string text = response.GetValue<string>();
                Console.WriteLine(text);
                // The socket.io server code looks like this:
                // socket.emit('hi', 'hi client');
            });

            connectionServer.On("rates", async (response) =>
            {
                string text = response.GetValue<string>();
                Console.WriteLine(text);
                try
                {
                    // connectionClient = new HubConnectionBuilder().WithUrl("https://localhost:7040/liveRatesHub", options =>
                    // {
                    //     options.Transports = HttpTransportType.WebSockets;
                    // }).WithAutomaticReconnect().Build();

                    // connectionClient.Closed += async (error) =>
                    // {
                    //     await Task.Delay(new Random().Next(0, 5) * 1000);
                    //     await connectionClient.StartAsync();
                    // };
                    await _liveRatesHub.Clients.All.SendAsync("ReceiveRates", text);
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
        }

        public async Task<string> GetRates()
        {
               
            try
            {
                using var client = new HttpClient();
                // Make the GET request to the Web API
                HttpResponseMessage response = await client.GetAsync(BASE_URL + END_POINT);

                // Check if the response is successful (status code in the range 200-299)
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Display the response
                    Console.WriteLine(responseBody);

                     //await _liveRatesHub.Clients.Caller.SendAsync("ReceiveRates", responseBody);
                    return responseBody;
                }
                else
                {
                    // Display the error status code
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Display any exceptions that occur
                Console.WriteLine($"An error occurred: {ex.Message}");
            }        
            return string.Empty;    
        }
    }
}