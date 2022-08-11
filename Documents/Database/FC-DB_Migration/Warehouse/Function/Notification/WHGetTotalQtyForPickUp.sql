CREATE FUNCTION [dbo].[WHGetTotalQtyForPickUp](@WarehouseID INT) RETURNS INTEGER
AS
BEGIN
	

	DECLARE @Total INT;


		SELECT @Total = 
		CASE 
			WHEN SUM(STClientDeliveries.Quantity) IS NULL THEN 0
			ELSE SUM(STClientDeliveries.Quantity)
			END
		FROM STORDERS AS STOrd
		JOIN STDeliveries ON STOrd.Id = STDeliveries.STOrderId
		JOIN STClientDeliveries ON STDeliveries.Id = STClientDeliveries.STDeliveryId
		Where 
			STOrd.DeliveryType = 1 
			AND STOrd.WarehouseId = @WarehouseID 
			AND STOrd.RequestStatus = 1
			AND STOrd.OrderType != 3
			AND ((SELECT COUNT(*) FROM STDeliveries as STDel JOIN STClientDeliveries as STCli
			on STCli.STDeliveryId = STDel.Id where STDel.STOrderId = STOrd.Id 
			AND (STCli.DeliveryStatus = 3 AND STCli.ReleaseStatus = 3)) > 0)
			AND ((SELECT COUNT(*) FROM STOrderDetails as STOrdD WHERE STOrdD.ReleaseStatus != 1 AND STOrdD.STOrderId = STOrd.Id) > 0);
	

	RETURN @Total;


END