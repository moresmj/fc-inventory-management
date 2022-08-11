CREATE FUNCTION [dbo].[WHGetOrdersWaitingForDeliveryCount](@WarehouseId int) RETURNS INTEGER

AS
BEGIN

	DECLARE @ClientDeliveryTotal as INT;
	DECLARE @ShowroomDeliveryTotal as INT;

	--GET Client Delivery pending total
	SELECT @ClientDeliveryTotal = 
		CASE
			WHEN COUNT(*) IS NULL THEN 0
			ELSE COUNT(*)
			END
		FROM STORDERS AS STOrd
		Where 
			STOrd.DeliveryType != 1 AND STOrd.WarehouseId = @WarehouseId  
			AND (SELECT COUNT(*) FROM STDeliveries as STDel JOIN STClientDeliveries as STCli
			on STCli.STDeliveryId = STDel.Id where STDel.STOrderId = STOrd.Id 
			AND (STCli.DeliveryStatus = 3 AND STCli.ReleaseStatus = 3)) > 0;

	--GET Showroom Delivery pending total
	 SELECT @ShowroomDeliveryTotal = 
		CASE
			WHEN COUNT(*) IS NULL THEN 0
			ELSE COUNT(*)
			END
		FROM STORDERS AS STOrd
		Where 
			STOrd.DeliveryType != 1 AND STOrd.WarehouseId = @WarehouseId  
			AND (SELECT COUNT(*) FROM STDeliveries AS STDel JOIN STShowroomDeliveries as STShow
			on STShow.STDeliveryId = STDel.Id where STDel.STOrderId = STOrd.Id 
			AND (STShow.DeliveryStatus = 3 AND STShow.ReleaseStatus = 3)) > 0



		RETURN @ClientDeliveryTotal + @ShowroomDeliveryTotal;

END
