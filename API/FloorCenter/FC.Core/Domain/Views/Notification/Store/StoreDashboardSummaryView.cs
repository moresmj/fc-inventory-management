using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FC.Core.Domain.Views.Notification.Store
{
    [NotMapped]
    public class StoreDashboardSummaryView
    {
        public int? Id { get; set; }

        public int? pendingOrdersTotal { get; set; }

	    public int? pendingOrdersTotalItem { get; set; }

	    public int? pendingDeliveriesTotal { get; set; }

	    public int? pendingDeliveriesTotalItem { get; set; }

	    public int? pendingToReceiveReturnsTotal { get; set; }

	    public int? pendingToReceiveReturnsTotalItems { get; set; }

	    public int? pendingSalesTotal { get; set; }
       
        public int? pendingSalesTotalItem {get; set;}

	    public int? waitingDeliveriesTotal {get; set;}

	    public int? waitingDeliveriesTotalItem {get; set;}

	    public int? waitingForPickUpTotal {get; set;}

        public int? waitingForPickUpTotalItem { get; set; }

        public int? NotificationsTotal { get; set; }
    }
}
