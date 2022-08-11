CREATE FUNCTION [dbo].[LogGetPendingPickUpRTVNotif]()
RETURNS @PickupRTVReturns TABLE
(
	Id INT,
	PendingPickUpRTVTotal INT,
	PendingPickUpRTVTotalItem INT

)

AS
	BEGIN


		DECLARE @ReturnsTable TABLE
		(
			Id int,
			RequestStatus int,
			ReturnType int,
			DeliveryStatus int,
			Quantity int
		)

		--Will get record and insert to temp table to be filtered
		INSERT @ReturnsTable

		SELECT STReturns.Id, 
				STReturns.RequestStatus, 
				STReturns.ReturnType,
				WHDeliveryDetails.DeliveryStatus, 
				WHDeliveryDetails.Quantity 
		FROM STReturns
		JOIN WHDeliveries as Deliveries
		ON STReturns.Id = Deliveries.STReturnId
		LEFT JOIN WHDeliveryDetails as WHDeliveryDetails
		ON Deliveries.Id = WHDeliveryDetails.WHDeliveryId
		WHERE ((STReturns.RequestStatus = 1 AND WHDeliveryDetails.DeliveryStatus =  2) 
		OR (STReturns.RequestStatus = 2 AND STReturns.ReturnType = 3) )

		INSERT @PickupRTVReturns
		SELECT 1 AS Id,
				COUNT(DISTINCT ID) AS PendingPickUpRTVTotal, 
				COALESCE(SUM(Quantity),0) AS PendingPickUpRTVTotalItem 
		FROM @ReturnsTable where DeliveryStatus  = 2
	

		RETURN;
	END






