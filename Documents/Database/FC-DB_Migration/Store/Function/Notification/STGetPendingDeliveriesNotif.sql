CREATE FUNCTION [dbo].[STGetPendingDeliveriesNotif](@StoreId INT)
RETURNS @PendingDeliveriesNotif TABLE
(
	Id INT,
	PendingDeliveriesTotal INT,
	PendingDeliveriesTotalItem INT

)

AS
	BEGIN
		DECLARE @ForDelivery TABLE (Id int,
									DeliveryStatus int,
									DeliveryQty int,
									RequestStatus int,
									WarehouseId int,
									StoreId int,
									ApprovedDeliveryDate datetime,
									DeliveryType int
									);

		INSERT INTO @ForDelivery
			SELECT * FROM dbo.GetDeliveryRecordsNotif(@StoreId,null,null)






			INSERT INTO @PendingDeliveriesNotif
			SELECT
				 1 as Id,
				 (SELECT COALESCE(COUNT(DISTINCT ID),0) FROM @ForDelivery WHERE DeliveryStatus = 2 AND RequestStatus = 1) AS PendingDeliveriesTotal,
				 (SELECT COALESCE(SUM(DeliveryQty),0) FROM @ForDelivery WHERE  DeliveryStatus = 2 AND RequestStatus = 1) AS PendingDeliveriesTotalItem


			 RETURN;
		 END