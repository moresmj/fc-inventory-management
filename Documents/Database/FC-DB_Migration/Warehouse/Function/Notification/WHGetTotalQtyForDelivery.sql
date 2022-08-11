CREATE FUNCTION [dbo].[WHGetTotalQtyForDelivery](@WarehouseID INT) RETURNS INTEGER
AS
BEGIN
	
	DECLARE @DelItems TABLE ([showQty] int,[clientQty] int);
	DECLARE @Total INT;


	
	INSERT INTO @DelItems (showQty,clientQty) 

	SELECT  
		(SELECT SUM(STShowDel.Quantity) 
		FROM STDeliveries AS STDel 
		JOIN STShowroomDeliveries AS STShowDel
		on STShowDel.STDeliveryId = STDel.Id 
		where STDel.STOrderId = stord.Id 
		AND (STShowDel.DeliveryStatus = 3 AND STShowDel.ReleaseStatus = 3))
		AS showQty,

		(SELECT SUM(STClientDel.Quantity) 
		FROM STDeliveries AS STDel 
		JOIN STClientDeliveries AS STClientDel
		ON STDel.Id = STClientDel.STDeliveryId
		where STDel.STOrderId = stord.Id 
		AND (STClientDel.DeliveryStatus = 3 AND STClientDel.ReleaseStatus = 3))
		as clientQty

	FROM STORDERS AS STOrd
	Where STOrd.DeliveryType != 1 AND STOrd.WarehouseId = @WarehouseID  

	SET @Total = (SELECT SUM(showQty) + SUM(clientQty) FROM @DelItems);
	

	RETURN @Total;


END