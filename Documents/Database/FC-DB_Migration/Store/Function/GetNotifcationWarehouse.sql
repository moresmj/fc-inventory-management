--get for delivery count
SELECT * FROM STOrders 
LEFT JOIN(
SELECT COUNT(*), Id FROM STORDERS stord
Where stord.DeliveryType != 1 AND stord.WarehouseId = 29  
AND (SELECT COUNT(*) FROM STDeliveries stdel JOIN STClientDeliveries as stc
on stc.STDeliveryId = stdel.Id where stdel.STOrderId = stord.Id 
AND (stc.DeliveryStatus = 3 AND stc.ReleaseStatus = 3)) > 0) as ClientOrders ON STOrders.Id = ClientOrders.Id
LEFT JOIN
--Get showroom delivery count
(
SELECT COUNT(*),Id FROM STORDERS stord
Where stord.DeliveryType != 1 AND stord.WarehouseId = 29  
AND (SELECT COUNT(*) FROM STDeliveries stdel JOIN STShowroomDeliveries as std
on std.STDeliveryId = stdel.Id where stdel.STOrderId = stord.Id 
AND (std.DeliveryStatus = 3 AND std.ReleaseStatus = 3)) > 0) as ShowroomOrders ON STOrders.Id = ShowroomOrders.Id



DECLARE @DelItems TABLE ([quantity] int);
DECLARE @TotalClient INT;
DECLARE @TotalShowroom INT;
INSERT INTO @DelItems (quantity) 
SELECT  (SELECT SUM(std.Quantity) FROM STDeliveries stdel JOIN STShowroomDeliveries as std
on std.STDeliveryId = stdel.Id where stdel.STOrderId = stord.Id 
AND (std.DeliveryStatus = 3 AND std.ReleaseStatus = 3)) as test
 FROM STORDERS stord
Where stord.DeliveryType != 1 AND stord.WarehouseId = 29  
AND (SELECT COUNT(*) FROM STDeliveries stdel JOIN STShowroomDeliveries as std
on std.STDeliveryId = stdel.Id where stdel.STOrderId = stord.Id 
AND (std.DeliveryStatus = 3 AND std.ReleaseStatus = 3)) > 0
(SELECT SUM(quantity) FROM @DelItems)
SET @totalShowroom = (SELECT SUM(quantity) FROM @DelItems);


DECLARE @DelItems2 TABLE ([quantity] int);
INSERT INTO @DelItems2 (quantity) 
SELECT  (SELECT SUM(stc.Quantity) FROM STDeliveries stdel JOIN STClientDeliveries as stc
on stc.STDeliveryId = stdel.Id where stdel.STOrderId = stord.Id 
AND (stc.DeliveryStatus = 3 AND stc.ReleaseStatus = 3)) as test
 FROM STORDERS stord
Where stord.DeliveryType != 1 AND stord.WarehouseId = 29  
AND (SELECT COUNT(*) FROM STDeliveries stdel JOIN STClientDeliveries as stc
on stc.STDeliveryId = stdel.Id where stdel.STOrderId = stord.Id 
AND (stc.DeliveryStatus = 3 AND stc.ReleaseStatus = 3)) > 0
(SELECT SUM(quantity) FROM @DelItems2);
SET @TotalClient = (SELECT SUM(quantity) FROM @DelItems2);

SELECT @TotalClient + @TotalShowroom


SELECT * FROM STOrders WHERE ID = 397
SELECT COUNT(*) FROM STDeliveries where STOrderId = 397


--Getting pickup count
		SELECT	COUNT(*)
		FROM STORDERS AS STOrd
		Where 
			STOrd.DeliveryType = 1 
			AND STOrd.WarehouseId = 29 
			AND STOrd.RequestStatus = 1
			AND STOrd.OrderType != 3
			AND ((SELECT COUNT(*) FROM STDeliveries as STDel JOIN STClientDeliveries as STCli
			on STCli.STDeliveryId = STDel.Id where STDel.STOrderId = STOrd.Id 
			AND (STCli.DeliveryStatus = 3 AND STCli.ReleaseStatus = 3)) > 0)
			AND ((SELECT COUNT(*) FROM STOrderDetails as STOrdD WHERE STOrdD.ReleaseStatus != 1 AND STOrdD.STOrderId = STOrd.Id) > 0);