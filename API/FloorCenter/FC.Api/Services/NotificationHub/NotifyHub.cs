using FC.Api.Helpers;
using FC.Core.Domain.Users;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.NotificationHub
{
    public class NotifyHub : Hub<ITypedHubClient>
    {

        public override async Task OnConnectedAsync()
        {
      
            var httpContext = Context.Connection.GetHttpContext();
            var groupNumber = httpContext.Request.Query["Assignment"];

            var test = Convert.ToInt32(groupNumber[0]);
            var groupName = EnumExtensions.SplitName(Enum.GetName(typeof(AssignmentEnum), test));


            await Groups.AddAsync(Context.ConnectionId, groupName);
            await base.OnConnectedAsync();

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var httpContext = Context.Connection.GetHttpContext();
            var group = httpContext.Request.Query["Assignment"];
            var groupName = EnumExtensions.SplitName(Enum.GetName(typeof(AssignmentEnum), Convert.ToInt32(group[0])));

            await Groups.RemoveAsync(Context.ConnectionId, groupName);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
