using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.NotificationHub
{
    public interface ITypedHubClient
    {
        Task BroadCastMessage(string type, string payload);

        Task BroadCastNotificationMain(string type, string payload);
    }
}
