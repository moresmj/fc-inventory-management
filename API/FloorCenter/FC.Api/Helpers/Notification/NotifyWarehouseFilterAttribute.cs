using FC.Api.Services.NotificationHub;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FC.Api.Helpers.Notification
{

    public class NotifyWarehouseFilterAttribute : ActionFilterAttribute
    {

        private IHubContext<NotifyHub, ITypedHubClient> _hubContext;
        protected ClaimsPrincipal currentUser;
        private DataContext _context;
        public string notifGroup { get; set; }

        public NotifyWarehouseFilterAttribute(IHubContext<NotifyHub,
            ITypedHubClient> hubContext,
            DataContext context,
            string NotifGroup)
        {
            _hubContext = hubContext;
            _context = context;

        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
          
            base.OnActionExecuted(context);
            try
            {
                //_hubContext.Clients.All.BroadCastMessage("success", "test");
                _hubContext.Clients.Group(notifGroup).BroadCastMessage("success", "test");
            }
            catch (Exception e)
            {
                e.ToString();
            }

        }




    }
}
