CREATE FUNCTION [dbo].[GetDeliveryRecordsNotif](@StoreId int, @WarehouseId int,@NotifType int)
RETURNS @ForDelivery TABLE
(
	Id int,
	DeliveryStatus int,
	DeliveryQty int,
	RequestStatus int,
	WarehouseId int,
	StoreId int,
	ApprovedDeliveryDate datetime,
	DeliveryType int
)
--@NotifType = 3 for storeId

AS
	BEGIN

		IF (@NotifType IS NULL)
			BEGIN
			INSERT @ForDelivery
				SELECT Deliveries.Id,
									CASE WHEN @NotifType = 3
											THEN  
												CASE 
													WHEN Orders.Id IS NOT NULL 
														THEN ShowroomDeliveries.DeliveryStatus
														ELSE NULL
												END
											ELSE
												CASE WHEN ClientDeliveries.DeliveryStatus IS NOT NULL 
														THEN ClientDeliveries.DeliveryStatus
														ELSE ShowroomDeliveries.DeliveryStatus 
												END 
									END as DeliveryStatus,

									CASE WHEN ClientDeliveries.Quantity	 IS NOT NULL 
										THEN ClientDeliveries.Quantity
										ELSE ShowroomDeliveries.Quantity END as DeliveryQty,

									Orders.RequestStatus,
									Orders.WarehouseId,
									Deliveries.StoreId,
									Deliveries.ApprovedDeliveryDate,
									CASE WHEN Orders.Id IS NOT NULL
										 THEN Orders.DeliveryType
										 ELSE 2 END as DeliveryType
					
								from STDeliveries as Deliveries

								LEFT JOIN STOrders as Orders
									ON Deliveries.STOrderId = Orders.Id

								LEFT JOIN Warehouses as Warehouse
									ON Orders.WarehouseId = Warehouse.Id

								LEFT JOIN STClientDeliveries as ClientDeliveries
									ON Deliveries.Id = ClientDeliveries.STDeliveryId

								LEFT JOIN STShowroomDeliveries as ShowroomDeliveries
									ON Deliveries.Id = ShowroomDeliveries.STDeliveryId

							 WHERE (
								 	(Orders.Id IS NOT NULL AND (Warehouse.Vendor IS  NULL OR Warehouse.Vendor = 0))
									 OR (Orders.Id IS NULL)
								   ) 
			END


		ELSE
			BEGIN
			INSERT @ForDelivery
				SELECT Deliveries.Id,
								CASE WHEN @NotifType = 3
										THEN  
											CASE 
												WHEN Orders.Id IS NOT NULL 
													THEN ShowroomDeliveries.DeliveryStatus
													ELSE NULL
											END
										ELSE
											CASE WHEN ClientDeliveries.DeliveryStatus IS NOT NULL 
													THEN ClientDeliveries.DeliveryStatus
													ELSE ShowroomDeliveries.DeliveryStatus 
											END 
								END as DeliveryStatus,

								CASE WHEN ClientDeliveries.Quantity	 IS NOT NULL 
									THEN ClientDeliveries.Quantity
									ELSE ShowroomDeliveries.Quantity END as DeliveryQty,

								Orders.RequestStatus,
								Orders.WarehouseId,
								Deliveries.StoreId,
								Deliveries.ApprovedDeliveryDate,
								CASE WHEN Orders.Id IS NOT NULL
									 THEN Orders.DeliveryType
									 ELSE 2 END as DeliveryType
					
							from STDeliveries as Deliveries

							LEFT JOIN STOrders as Orders
								ON Deliveries.STOrderId = Orders.Id

							LEFT JOIN Warehouses as Warehouse
								ON Orders.WarehouseId = Warehouse.Id

							LEFT JOIN STClientDeliveries as ClientDeliveries
								ON Deliveries.Id = ClientDeliveries.STDeliveryId

							LEFT JOIN STShowroomDeliveries as ShowroomDeliveries
								ON Deliveries.Id = ShowroomDeliveries.STDeliveryId

						 WHERE (
						 	 (Orders.Id IS NOT NULL AND (Warehouse.Vendor IS  NULL OR Warehouse.Vendor = 0))
							 OR (Orders.Id IS NULL)
							 AND (Orders.WarehouseId = @WarehouseId OR Deliveries.StoreId = @StoreId)
							 ) 

			END

			RETURN;

		END


