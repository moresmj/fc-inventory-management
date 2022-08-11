using System;
using System.Collections.Generic;
using System.Text;

namespace FC.Core.Domain.Views
{
    public class MainDashboardSummaryView
    {

        public int? Id { get; set; }

        public int? approveRequestItemTotal { get; set; }

        public int? PendingTransferTotalItem { get; set; }

        public int? PendingAssignDrTotalItem { get; set; }

        public int? StoreAdjustmentTotalItem { get; set; }

        public int? PendingReturnsTotalItem { get; set; }

    }
}
