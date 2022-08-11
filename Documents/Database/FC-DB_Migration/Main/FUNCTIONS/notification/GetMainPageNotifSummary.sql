CREATE FUNCTION [dbo].[GetMainPageNotifSummary]()
RETURNS @MainPageNotifSummary TABLE
(
	id int,
	approveRequestTotal int,
	storeAdjustmentTotal int,
	pendingTransferTotal int,
	pendingReturnsTotal int,
	pendingAssignDrTotal int,
	notificationsTotal int
)

AS
	BEGIN
		INSERT @MainPageNotifSummary
			SELECT 1 as id,
				   (SELECT ApproveRequestTotal FROM dbo.GetApproveRequestSummary()) as approveRequestTotal,
				   (SELECT StoreAdjustmentTotal FROM dbo.GetAdjustmentStore()) as storeAdjustmentTotal,
				   (SELECT PendingTransferTotal FROM dbo.GetPendingTransfersNotifcation()) as pendingTransferTotal,
				   (SELECT pendingReturnsTotal FROM dbo.GetReturnPendingSummary()) as pendingTransferTotal,
				   (SELECT PendingAssignDrTotal FROM dbo.GetPendingAssignDrNotifcation()) as pendingAssignDrTotal,
				   0 as notificationsTotal

		RETURN;
	END
