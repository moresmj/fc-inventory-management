CREATE FUNCTION [dbo].[WHGetTotalItemForReleasing](@WarehouseId int, @ItemId int) RETURNS INTEGER
AS
BEGIN
	DECLARE @Total INTEGER;

	-- CREATED TEMP TABLE FOR STORING THE APPROVED QUANTITY AND RELEASED QUANTITY ON SHOWROOMDELIVERIES AND CLIENT DELIVERIES
	DECLARE @ForReleaseTempTable TABLE (ApprovedQuantity int, DeliveryQuantity int);

	-- FETCHING OF APPROVED QUANTITY AND RELEASED QUANTITY ON SHOWROOM DELIVERIES AND CLIENT DELIVERIES
	-- Reference : http://prntscr.com/nfkccc
	--STO.WHDRNumber IS NOT NULL and  removed condition to reflect the order as for releasing after creating a PO
	INSERT INTO @ForReleaseTempTable	
	SELECT 
	--Added for ticket 795 will compute approve quantity if order is already approved
	CASE
		WHEN STO.RequestStatus = 1 THEN
			(SELECT SUM(ApprovedQuantity) FROM STOrderDetails WHERE STOrderId = STO.Id and ItemId = @ItemId and (ReleaseStatus = 2 OR ReleaseStatus = 3))
		WHEN STO.RequestStatus = 2 THEN
			(SELECT SUM(RequestedQuantity) FROM STOrderDetails WHERE STOrderId = STO.Id and ItemId = @ItemId and (ReleaseStatus = 2 OR ReleaseStatus = 3))
		END as ApprovedQuantity,
		
		CASE 
			WHEN STO.OrderType = 1 THEN	

				(SELECT COALESCE
				((SELECT SUM(STSD.Quantity)
					FROM STDeliveries as STD LEFT JOIN  STShowroomDeliveries as STSD ON STD.Id = STSD.STDeliveryId 
					WHERE STD.STOrderId = STO.Id and STD.IsRemainingForReceivingDelivery = 0 and STSD.ItemId = @ItemId AND STSD.ReleaseStatus = 1), 0)
				)

			WHEN STO.OrderType = 2 and STO.DeliveryType = 1 THEN
			(SELECT COALESCE
				((SELECT SUM(STCD.Quantity)
				    FROM STDeliveries as STD LEFT JOIN STClientDeliveries as STCD ON STD.Id = STCD.STDeliveryId 
					WHERE STD.STOrderId = STO.Id and STCD.ReleaseStatus = 1 and STCD.ItemId = @ItemId), 0)
			 )

			WHEN STO.OrderType = 2 and STO.DeliveryType = 2 THEN
			(SELECT COALESCE
				((SELECT SUM(STCD.Quantity)
					FROM STDeliveries as STD LEFT JOIN STClientDeliveries as STCD ON STD.Id = STCD.STDeliveryId 
					WHERE STD.STOrderId = STO.Id and STCD.ReleaseStatus = 1 and STCD.ItemId = @ItemId),0)
			)

			WHEN STO.OrderType = 2 and STO.DeliveryType = 3 THEN
			(SELECT COALESCE
				((SELECT SUM(STSD.Quantity)
					FROM STDeliveries as STD LEFT JOIN  STShowroomDeliveries as STSD ON STD.Id = STSD.STDeliveryId 
					WHERE STD.STOrderId = STO.Id and STD.IsRemainingForReceivingDelivery = 0 and STSD.ItemId = @ItemId AND STSD.ReleaseStatus = 1),0)
			)
				
			END as ForReleasing
			
		FROM STOrders as STO LEFT JOIN STOrderDetails as STOD ON STO.Id = STOD.STOrderId
		WHERE 
		-- change request status from approved to pending, and release and delivery status from waitng to pending
			STO.WarehouseId = @WarehouseId and ItemId = @ItemId and
			((STO.RequestStatus = 2 OR STO.RequestStatus = 1) 
			and ((STOD.DeliveryStatus = 2 and STOD.ReleaseStatus = 2) 
			OR  ( STOD.DeliveryStatus = 3 and STOD.ReleaseStatus = 3)));


		SELECT @Total =
			(CASE 
				WHEN SUM(ApprovedQuantity) IS NULL THEN 0 
				ELSE SUM(ApprovedQuantity)
			END 
			-
			CASE 
				WHEN SUM(DeliveryQuantity) IS NULL THEN 0 
				ELSE SUM(DeliveryQuantity)
			END) 
		FROM @ForReleaseTempTable ;

		RETURN @Total
END