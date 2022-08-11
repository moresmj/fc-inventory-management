CREATE FUNCTION [dbo].[LogGetPendingDeliverySalesNotif]()
RETURNS @PendingSalesDelivery TABLE
(
	Id INT,
	PendingSalesDeliveryTotal INT,
	PendingSalesDeliveryTotalItem INT

)

AS
	BEGIN
		INSERT @PendingSalesDelivery
			SELECT  1 as Id,
					COUNT(DISTINCT Deliveries.Id) as PendingSalesDeliveryTotal,
				    COALESCE(SUM(ClientDeliveries.Quantity),0) as PendingSalesDeliveryTotalItem
					 FROM STDeliveries as Deliveries
					 LEFT JOIN STClientDeliveries AS ClientDeliveries
					 ON Deliveries.Id = ClientDeliveries.STDeliveryId
					 WHERE (Deliveries.STSalesId IS NOT NULL AND ClientDeliveries.DeliveryStatus = 2)

			RETURN;
	END


