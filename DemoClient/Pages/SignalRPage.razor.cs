using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DemoClient.Pages
{
    public partial class SignalRPage
    {
        [Inject]
        private IConfiguration Cfg { get; set; }

        [Inject]
        private ILoggerProvider LoggerProvider { get; set; }


        List<string> messages = new List<string>();
        string httpResponse = "";

        private HubConnection hubConnection;

        protected override async Task OnInitializedAsync()
        {
            var signalREndpoint = Cfg.GetValue<string>("Endpoints:SignalR")
                ?? throw new ArgumentException("The configuration setting Endpoints:SignalR was not defined.");

            hubConnection = new HubConnectionBuilder()
                .WithUrl(signalREndpoint)
                .ConfigureLogging(logger => logger.AddProvider(LoggerProvider))
                .Build();

            hubConnection.On<string>("ReceiveMessage", (message) =>
            {
                messages.Add(message);
                StateHasChanged();
            });

            await hubConnection.StartAsync();
        }

        public async Task JoinGroup()
        {
            var signalREndpoint = Cfg.GetValue<string>("Endpoints:SignalR")
                ?? throw new ArgumentException("The configuration setting Endpoints:SignalR was not defined.");
            var id = hubConnection.ConnectionId;
            var req = new HttpRequestMessage(HttpMethod.Post, $"{signalREndpoint}/addtogroup/demogroup/{id}");
            
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(req);
            httpResponse = $"Status code: {response.StatusCode}, message: {await response.Content.ReadAsStringAsync()}";

            StateHasChanged();
        }

        public async Task LeaveGroup()
        {
            await hubConnection.SendAsync("RemoveFromGroup", "demogroup", hubConnection.ConnectionId);
        }

        public async Task LeaveAllGroups()
        {
            await hubConnection.SendAsync("RemoveFromAllGroups", hubConnection.ConnectionId);
        }
    }
}