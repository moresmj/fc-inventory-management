using System;
using System.Collections.Generic;
using System.Text;

namespace FC.Core.Domain.Views.Notification
{
    public class LogisticsNotifSummaryView
    {

       public int? Id { get; set; }

       public int? pendingOrderDeliveriesTotal { get; set; }

       public int? pendingOrderDeliveriesTotalItem { get; set; }

	   public int? waitingOrderDeliveriesTotal { get; set; }

       public int? waitingDeliveriesTotalItem { get; set; }

	   public int? pendingSalesDeliveryTotal { get; set; }

       public int? pendingSalesDeliveryTotalItem { get; set; }

	   public int? pendingPickUpClientReturnsTotal { get; set; }

       public int? pendingPickUpClientReturnsTotalItem { get; set; }

	   public int? pendingPickUpRTVTotal { get; set; }

       public int? pendingPickUpRTVTotalItem { get; set; }

	   public int? notificationsTotal { get; set; }

    }
}
