CREATE FUNCTION [dbo].[STGetPendingToReceiveReturnsNotif](@StoreId INT)
RETURNS @PendingToReceiveReturns TABLE
(
	Id INT,
	PendingToReceiveReturnsTotal INT,
	PendingToReceiveReturnsTotalItems INT
)

AS
	BEGIN
		DECLARE @PendingReturnsTable TABLE (id int, ReturnType int, ClientReturnType int, DeliveryStatus int, Quantity int)

		--Will get pending returns and insert to @PendingReturnsTable
		INSERT INTO @PendingReturnsTable
			SELECT STReturns.Id,
					STReturns.ReturnType,
					STReturns.ClientReturnType,
					ClientReturns.DeliveryStatus,
					ClientReturns.Quantity 
			FROM STReturns
			LEFT JOIN STClientReturns AS ClientReturns
			ON STReturns.Id = ClientReturns.STReturnId
			WHERE STReturns.StoreId = @StoreId AND STReturns.ReturnType = 2

		--Will count id and quantity to be returned
		INSERT @PendingToReceiveReturns
			SELECT 1 as Id,
					COUNT(DISTINCT id) AS PendingToReceiveReturnsTotal, 
					COALESCE(SUM(Quantity),0) AS Quantity FROM @PendingReturnsTable
			WHERE (ClientReturnType = 1 AND DeliveryStatus = 2) OR ( ClientReturnType != 1 AND DeliveryStatus = 3)


			RETURN;


	END



