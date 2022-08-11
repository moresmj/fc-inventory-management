CREATE FUNCTION [dbo].[WHGetOrdersWaitingForPickupCount](@WarehouseId int) RETURNS INTEGER

AS
BEGIN

	DECLARE @Total as INT;

	--GET Client Delivery pending total
	SELECT @Total = 
		CASE
			WHEN COUNT(*) IS NULL THEN 0
			ELSE COUNT(*)
			END
		FROM STORDERS AS STOrd
		Where 
			STOrd.DeliveryType = 1 
			AND STOrd.WarehouseId = @WarehouseId 
			AND STOrd.RequestStatus = 1
			AND STOrd.OrderType != 3
			AND ((SELECT COUNT(*) FROM STDeliveries as STDel JOIN STClientDeliveries as STCli
			on STCli.STDeliveryId = STDel.Id where STDel.STOrderId = STOrd.Id 
			AND (STCli.DeliveryStatus = 3 AND STCli.ReleaseStatus = 3)) > 0)
			AND ((SELECT COUNT(*) FROM STOrderDetails as STOrdD WHERE STOrdD.ReleaseStatus != 1 AND STOrdD.STOrderId = STOrd.Id) > 0);

		RETURN @Total;

END