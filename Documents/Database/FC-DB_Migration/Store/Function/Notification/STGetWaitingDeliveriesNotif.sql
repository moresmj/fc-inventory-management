CREATE FUNCTION [dbo].[STGetWaitingDeliveriesNotif](@StoreId INT)
RETURNS @WaitingDeliveriesNotif TABLE
(
	Id INT,
	WaitingDeliveriesTotal INT,
	WaitingDeliveriesTotalItem INT

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
			SELECT * FROM dbo.GetDeliveryRecordsNotif(@StoreId,null,3)






			INSERT INTO @WaitingDeliveriesNotif
			SELECT
				 1 as Id,
				 (SELECT COALESCE(COUNT(DISTINCT ID),0) FROM @ForDelivery WHERE (DeliveryStatus = 3 AND RequestStatus = 1 AND ApprovedDeliveryDate IS NOT NULL 
																				AND (DeliveryType = 2 OR DeliveryType = 3))) AS WaitingDeliveriesTotal,

				 (SELECT COALESCE(SUM(DeliveryQty),0) FROM @ForDelivery WHERE  (DeliveryStatus = 3 AND RequestStatus = 1 AND ApprovedDeliveryDate IS NOT NULL 
																				AND (DeliveryType = 2 OR DeliveryType = 3))) AS WaitingDeliveriesTotalItem


			 RETURN;
		 END