CREATE FUNCTION [dbo].[GetPendingTransfersNotifcation]()
RETURNS @ForApprovalTransferSummary TABLE
(
	PendingTransferTotal int,
	PendingTransferTotalItem int
)

AS
BEGIN
	INSERT @ForApprovalTransferSummary
			SELECT COUNT(DISTINCT STOrders.Id) as PendingTransferTotal,
			COALESCE(SUM(STOrderDetails.RequestedQuantity),0) as PendingTransferTotalItem
			 FROM STOrders 
			LEFT JOIN STOrderDetails
			ON STOrders.Id = STOrderDetails.STOrderId
			WHERE OrderType = 3 AND RequestStatus = 2
	RETURN

END