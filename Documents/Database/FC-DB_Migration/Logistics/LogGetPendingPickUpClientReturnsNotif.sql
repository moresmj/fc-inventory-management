CREATE FUNCTION [dbo].[LogGetPendingPickUpClientReturns]()
RETURNS @PickupClientReturns TABLE
(
	Id INT,
	PendingPickUpClientReturnsTotal INT,
	PendingPickUpClientReturnsTotalItem INT

)

AS
	BEGIN
		INSERT @PickupClientReturns
			SELECT 1 as Id,
				   COUNT(DISTINCT STReturn.Id) as PendingSalesDeliveryTotal,
				   COALESCE(SUM(ClientReturns.Quantity),0) as PendingSalesDeliveryTotalItem
					FROM STReturns as STReturn
					LEFT JOIN STClientReturns as ClientReturns
					ON STReturn.Id = ClientReturns.STReturnId
					WHERE STReturn.ReturnType = 2 AND STReturn.ClientReturnType = 2 AND ClientReturns.DeliveryStatus = 2

			RETURN;
	END



