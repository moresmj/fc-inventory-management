using FC.Api.Services.NotificationHub;
using FC.Core.Domain.Users;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Helpers.Notification
{
    public class NotifyActionFilter : TypeFilterAttribute
    {

        public NotifyActionFilter(params AssignmentEnum[] groupNum) : base(typeof(NotifyActionFilterImp))
        {
            Arguments = new object[] { groupNum };
        }

        private class NotifyActionFilterImp : IActionFilter
        {

            private readonly AssignmentEnum[] _groupNum;
            private IHubContext<NotifyHub, ITypedHubClient> _hubContext;

            public NotifyActionFilterImp(AssignmentEnum[] groupNum, IHubContext<NotifyHub, ITypedHubClient> hubContext)
            {
                _groupNum = groupNum;
                _hubContext = hubContext;

            }

            public void OnActionExecuting(ActionExecutingContext context)
            {

               

            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
                try
                {
                    var userGroup = Convert.ToInt32(_groupNum[0]);
                    var notifyGroup = EnumExtensions.SplitName(Enum.GetName(typeof(AssignmentEnum), userGroup));
                    //Will get notification base on users group main,warehouse, store etc
                    _hubContext.Clients.Group(notifyGroup).BroadCastMessage("success", "notification update");
                }
                catch (Exception e)
                {
                    e.ToString();
                }

            }

        }
    }
}
