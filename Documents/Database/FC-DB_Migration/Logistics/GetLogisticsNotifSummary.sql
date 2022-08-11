CREATE FUNCTION [dbo].[GetLogisticsNotifSummary]()
RETURNS @LogisticsNotifSummary TABLE
(
	Id int PRIMARY KEY NOT NULL, 
    pendingOrderDeliveriesTotal  INT,
	pendingOrderDeliveriesTotalItem INT,
	waitingOrderDeliveriesTotal INT,
	waitingDeliveriesTotalItem INT,
	pendingSalesDeliveryTotal INT,
	pendingSalesDeliveryTotalItem INT,
	pendingPickUpClientReturnsTotal INT,
	pendingPickUpClientReturnsTotalItem INT,
	pendingPickUpRTVTotal INT,
	pendingPickUpRTVTotalItem INT,
	notificationsTotal INT
)

AS
BEGIN
		INSERT @LogisticsNotifSummary
		SELECT
				1 AS Id,
				PendingDeliveryOrder.pendingOrderDeliveriesTotal,
				PendingDeliveryOrder.pendingOrderDeliveriesTotalItem,

				PendingDeliveryOrder.waitingOrderDeliveriesTotal,
				PendingDeliveryOrder.waitingDeliveriesTotalItem,

				PendingSalesDelivery.pendingSalesDeliveryTotal,
				PendingSalesDelivery.pendingSalesDeliveryTotalItem,

				PendingPickupClientReturns.pendingPickUpClientReturnsTotal,
				PendingPickupClientReturns.pendingPickUpClientReturnsTotalItem,

				PendingRtv.pendingPickUpRTVTotal,
				PendingRtv.pendingPickUpRTVTotalItem,

				PendingDeliveryOrder.pendingOrderDeliveriesTotal 
				+ PendingSalesDelivery.pendingSalesDeliveryTotal
				+ PendingPickupClientReturns.pendingPickUpClientReturnsTotal
				+ PendingRtv.pendingPickUpRTVTotal AS notificationsTotal

			
			FROM dbo.LogGetPendingDeliveryOrdersNotif() AS PendingDeliveryOrder
				JOIN dbo.LogGetPendingDeliverySalesNotif() AS PendingSalesDelivery
					ON PendingDeliveryOrder.Id = PendingSalesDelivery.Id

				JOIN dbo.LogGetPendingPickUpClientReturns() AS PendingPickupClientReturns
					ON PendingDeliveryOrder.Id = PendingPickupClientReturns.Id

				JOIN dbo.LogGetPendingPickUpRTVNotif() AS PendingRtv
					ON PendingDeliveryOrder.Id = PendingRtv.Id

			RETURN
END


