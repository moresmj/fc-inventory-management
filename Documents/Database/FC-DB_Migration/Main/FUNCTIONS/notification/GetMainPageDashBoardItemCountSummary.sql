
CREATE FUNCTION [dbo].[GetMainPageDashBoardItemCountSummary]()
RETURNS @MainPageNotifSummary TABLE
(
	id int,
	approveRequestItemTotal int,
	StoreAdjustmentTotalItem int,
	PendingTransferTotalItem int,
	PendingReturnsTotalItem int,
	PendingAssignDrTotalItem int
)

AS
	BEGIN
		INSERT @MainPageNotifSummary
			SELECT 1 as id,

				   (SELECT approveRequestItemTotal FROM dbo.GetApproveRequestSummary()) as approveRequestItemTotal,
				   (SELECT StoreAdjustmentTotalItem FROM dbo.GetAdjustmentStore()) as StoreAdjustmentTotalItem,
				   (SELECT PendingTransferTotalItem FROM dbo.GetPendingTransfersNotifcation()) as PendingTransferTotalItem,
				   (SELECT PendingReturnsTotalItem FROM dbo.GetReturnPendingSummary()) as PendingReturnsTotalItem,
				   (SELECT PendingAssignDrTotalItem FROM dbo.GetPendingAssignDrNotifcation()) as PendingAssignDrTotalItem

		RETURN;
	END
