using Microsoft.Azure.Functions.Worker;
using SignalRDemo.DataTypes;

namespace SignalRDemo
{
    internal class ConnectionFunctions
    {
        /// <summary>
        /// This function is triggered whenever a client connects to the SignalR service.
        /// </summary>
        /// <param name="test">Some data</param>
        /// <returns>A notification message about the client that connected.</returns>
        [Function("onconnected")]
        [SignalROutput(HubName = "demohub")]
        public static SignalRMessage OnConnected(
            [SignalRTrigger(hubName: "demohub", category: "connections", @event: "connected")]
            InvocationContext context)
        {
            return new SignalRMessage
            {
                Target = "ReceiveMessage",
                Arguments = new object[] { $"Client with connection ID {context.ConnectionId} has connected." }
            };
        }

        /// <summary>
        /// This function is triggered whenever a client disconnects from the SignalR service.
        /// 
        /// Functions can also return SignalR outputs directly.
        /// </summary>
        /// <param name="test">Some data</param>
        /// <returns>A notification message about the client that disconnected.</returns>
        [Function("ondisconnected")]
        [SignalROutput(HubName = "demohub")]
        public static SignalRMessage OnDisconnected(
            [SignalRTrigger(hubName: "demohub", category: "connections", @event: "disconnected")]
            InvocationContext context)
        {
            return new SignalRMessage
            {
                Target = "ReceiveMessage",
                Arguments = new object[] { $"Client with connection ID {context.ConnectionId} has disconnected." }
            };
        }
    }
}