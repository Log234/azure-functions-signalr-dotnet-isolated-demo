using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using SignalRDemo.DataTypes;
using System.Net;
using System.Threading.Tasks;

namespace SignalRDemo
{
    public static class NegotiationFunctions
    {
        /// <summary>
        /// A Negotiate implementation for demonstrating how to accept a deserialized
        /// ConnectionInfo object.
        ///
        /// For demonstration purposes this function has authorization level anonymous.
        /// Make sure to properly protect your endpoints when implementing your SignalR project.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="connectionInfo">A deserialized ConnectionInfo object.</param>
        /// <returns></returns>
        [Function("negotiate")]
        public static async Task<HttpResponseData> Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            [SignalRConnectionInfoInput(HubName = "demohub")] ConnectionInfo connectionInfo)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");

            var originalConnectionInfo = JsonConvert.SerializeObject(connectionInfo);
            await response.WriteStringAsync(originalConnectionInfo);

            return response;
        }

        /// <summary>
        /// A much simpler implementation, for when you are not interested in the ConnectionInfo object.
        ///
        /// For demonstration purposes this function has authorization level anonymous.
        /// Make sure to properly protect your endpoints when implementing your SignalR project.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="connectionInfo">A serialized ConnectionInfo object.</param>
        /// <returns>A serialized ConnectionInfo object for the client as a HttpResponse.</returns>
        [Function("negotiate2")]
        public static async Task<HttpResponseData> NegotiateSimple(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            [SignalRConnectionInfoInput(HubName = "demohub")] string connectionInfo)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(connectionInfo);

            return response;
        }
    }
}