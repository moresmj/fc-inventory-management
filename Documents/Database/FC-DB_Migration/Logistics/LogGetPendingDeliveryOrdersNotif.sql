CREATE FUNCTION [dbo].[LogGetPendingDeliveryOrdersNotif]()
RETURNS @PendingDeliveryOrders TABLE
(
	Id int,
	PendingOrderDeliveriesTotal int,
	PendingOrderDeliveriesTotalItem int,
	WaitingOrderDeliveriesTotal int,
	WaitingDeliveriesTotalItem int
)


AS
	BEGIN

		DECLARE @ForDelivery TABLE
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
		INSERT @ForDelivery
			SELECT * FROM GetDeliveryRecordsNotif(null,null,null)

		INSERT @PendingDeliveryOrders
			SELECT 
				1 AS Id,
				(SELECT COUNT(DISTINCT id) from @ForDelivery WHERE (ApprovedDeliveryDate IS NULL AND RequestStatus = 1)) as PendingOrderDeliveriesTotal,
				(SELECT COALESCE(SUM(DeliveryQty),0) from @ForDelivery WHERE ApprovedDeliveryDate IS NULL) as PendingOrderDeliveriesTotalItem,
				(SELECT COUNT(DISTINCT id) from @ForDelivery WHERE (ApprovedDeliveryDate IS NOT NULL AND (RequestStatus = 1 OR RequestStatus IS NULL)) ) as WaitingOrderDeliveriesTotal,
				(SELECT COALESCE(SUM(DeliveryQty),0) from @ForDelivery WHERE ApprovedDeliveryDate IS NOT NULL) as WaitingDeliveriesTotalItem 

			RETURN
		END
