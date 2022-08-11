CREATE FUNCTION [dbo].[GetPendingOrdersNotif](@StoreId INT)
RETURNS @PendingOrders TABLE
(
	Id INT,
	PendingOrdersTotal INT,
	PendingOrdersTotalItem INT
)
AS
	BEGIN
		INSERT @PendingOrders
		SELECT 
				1 as Id,
				COUNT(DISTINCT Orders.Id) AS PendingOrdersTotal, 
				SUM(Details.RequestedQuantity) AS PendingOrdersTotalItem 
			FROM STOrders AS Orders
			LEFT JOIN STOrderDetails AS Details
			ON Orders.Id = Details.STOrderId
			WHERE Orders.StoreId = @StoreId AND Orders.RequestStatus = 2

		RETURN;

	END