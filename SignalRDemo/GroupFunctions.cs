using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SignalRDemo.DataTypes;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace SignalRDemo
{
    internal class GroupFunctions
    {
        /// <summary>
        /// Add a user to a SignalR group from an HTTP trigger. Also demonstrates how
        /// a function can return both a HttpResponse and a SignalR output.
        ///
        /// For demonstration purposes this function has authorization level anonymous.
        /// Make sure to properly protect your endpoints when implementing your SignalR project.
        /// </summary>
        /// <param name="req">Http request</param>
        /// <param name="groupName">Name of the group the user should be added to</param>
        /// <param name="connectionId">Connection ID of the user who should be added to the group</param>
        /// <returns></returns>
        [Function("addtogroup")]
        public static async Task<MultipleOutput> AddToGroup(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "addtogroup/{groupName}/{connectionId}")] HttpRequestData req,
            string groupName,
            string connectionId)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"Adding {connectionId} to {groupName}.");

            var addToGroupAction = new SignalRGroupAction
            {
                ConnectionId = connectionId,
                GroupName = groupName,
                Action = GroupAction.Add
            };

            var userJoinedNotification = new SignalRMessage
            {
                Target = "ReceiveMessage",
                GroupName = groupName,
                Arguments = new object[] { $"A user with connection ID {connectionId} has joined the group." }
            };

            var output = new MultipleOutput();
            output.GroupActions.Add(addToGroupAction);
            output.Messages.Add(userJoinedNotification);
            output.HttpResponse = response;

            return output;
        }

        /// <summary>
        /// Remove a user from a SignalR group via a SignalR trigger. Demonstrating how to send
        /// both a message and a group action at once.
        /// </summary>
        /// <param name="groupName">Name of the group the user should be removed from</param>
        /// <param name="connectionId">Connection ID of the user who should be removed fom the group</param>
        /// <returns></returns>
        [Function("removefromgroup")]
        public static MultipleOutput RemoveFromGroup(
            [SignalRTrigger(hubName: "demohub", category: "messages", @event: "RemoveFromGroup")]
            SignalRInput data)
        {
            string groupName = JsonSerializer.Deserialize<string>(data.Arguments[0].GetRawText());
            string connectionId = JsonSerializer.Deserialize<string>(data.Arguments[1].GetRawText());

            var removeFromGroup = new SignalRGroupAction
            {
                ConnectionId = connectionId,
                GroupName = groupName,
                Action = GroupAction.Remove
            };

            var notificationMessage = new SignalRMessage
            {
                Target = "ReceiveMessage",
                GroupName = groupName,
                Arguments = new object[] { $"The user with connection ID {connectionId} has left the group." }
            };

            var output = new MultipleOutput();
            output.GroupActions.Add(removeFromGroup);
            output.Messages.Add(notificationMessage);

            return output;
        }

        /// <summary>
        /// Remove a user from all SignalR groups via a SignalR trigger.
        /// </summary>
        /// <param name="connectionId">Connection ID of the user who should be removed fom the group</param>
        /// <returns></returns>
        [Function("removefromallgroups")]
        public static MultipleOutput RemoveFromAllGroups(
            [SignalRTrigger(hubName: "demohub", category: "messages", @event: "RemoveFromAllGroups")]
            SignalRInput data)
        {
            var removeFromAllGroups = new SignalRGroupAction
            {
                ConnectionId = data.ConnectionId,

                // Even though we remove the user from all groups we must still specify a group name.
                // This is due to the group name being a required JSON property, even though it will not be used.
                GroupName = "ThisIsAPlaceHolderBecauseSignalRsDataTypesAreAwkward",
                Action = GroupAction.RemoveAll
            };

            var notificationMessage = new SignalRMessage
            {
                Target = "ReceiveMessage",
                GroupName = "demogroup",
                Arguments = new object[] { $"The user with connection ID {data.ConnectionId} has left the group." }
            };

            var output =  new MultipleOutput();
            output.GroupActions.Add(removeFromAllGroups);
            output.Messages.Add(notificationMessage);

            return output;
        }
    }

    internal class MultipleOutput
    {
        [SignalROutput(HubName = "demohub")]
        public List<object> GroupActions { get; set; } = new List<object>();

        [SignalROutput(HubName = "demohub")]
        public List<SignalRMessage> Messages { get; set; } = new List<SignalRMessage>();

        public HttpResponseData HttpResponse { get; set; }
    }
}