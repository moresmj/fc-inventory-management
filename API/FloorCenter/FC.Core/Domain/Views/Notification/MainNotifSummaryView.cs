using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FC.Core.Domain.Views.Notification
{
    [NotMapped]
    public class MainNotifSummaryView
    {
        public int? Id { get; set; }

        public int? approveRequestTotal { get; set; }
                    
        public int? pendingTransferTotal { get; set; }
                     
        public int? pendingAssignDrTotal { get; set; }

        public int? storeAdjustmentTotal { get; set; }

        public int? pendingReturnsTotal { get; set; }

        public int? NotificationsTotal { get; set; }
    }
}
