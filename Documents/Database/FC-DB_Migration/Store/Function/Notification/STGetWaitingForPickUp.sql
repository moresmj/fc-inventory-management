CREATE FUNCTION [dbo].[STGetWaitingForPickUp](@StoreId INT)
RETURNS @WaitingOrders TABLE
(
	Id INT,
	WaitingForPickUpTotal INT,
	WaitingForPickUpTotalItem INT
)
AS
	BEGIN
		INSERT @WaitingOrders
		SELECT 
				1 as Id,
				COUNT(DISTINCT Orders.Id) AS PendingOrdersTotal, 
				COALESCE(SUM(Details.RequestedQuantity),0) AS PendingOrdersTotalItem 
			FROM STOrders AS Orders
			LEFT JOIN STOrderDetails AS Details
			ON Orders.Id = Details.STOrderId
			WHERE Orders.StoreId = @StoreId AND Orders.RequestStatus = 1
				  AND Orders.DeliveryType = 1 AND Details.ReleaseStatus = 3

		RETURN;

	END