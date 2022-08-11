using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FC.Core.Domain.Views
{
    [NotMapped]
    public class WarehouseNotifSummary 
    {
       public int? Id { get; set; }

       public int? WaitingDeliveriesTotal { get; set; }

       public int? WaitingDeliveriesTotalItem { get; set; }

       public int? WaitingForPickUpTotal { get; set; }

	   public int? WaitingForPickUpTotalItem { get; set; }

       public int? NotificationsTotal { get; set; }
    }
}
