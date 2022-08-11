CREATE FUNCTION [dbo].[GetPendingAssignDrNotifcation]()
RETURNS @ForAssignDrRequest TABLE
(
	PendingAssignDrTotal int,
	PendingAssignDrTotalItem int
)


AS
	BEGIN
		INSERT @ForAssignDrRequest
		SELECT COUNT(DISTINCT STOrders.Id) as PendingAssignDrTotal,  COALESCE(SUM(STOrderDetails.ApprovedQuantity),0) as PendingAssignDrTotalItem
		FROM STOrders 
		LEFT JOIN STOrderDetails
		ON STOrderDetails.STOrderId = STOrders.Id
		Where (OrderType = 1 OR OrderType = 2)
		AND RequestStatus = 1 AND WHDRNumber IS NULL

		RETURN;
	END


