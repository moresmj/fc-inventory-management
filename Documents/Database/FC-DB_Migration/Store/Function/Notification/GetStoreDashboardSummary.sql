CREATE FUNCTION [dbo].[GetStoreDashboardSummary](@StoreID int)
RETURNS @StoreNotifSummary TABLE
(
	Id int PRIMARY KEY NOT NULL, 
	pendingOrdersTotal int,
	pendingOrdersTotalItem int,
	pendingDeliveriesTotal int,
	pendingDeliveriesTotalItem int,
	pendingToReceiveReturnsTotal int,
	pendingToReceiveReturnsTotalItems int,
	pendingSalesTotal int,
	pendingSalesTotalItem int,
	waitingDeliveriesTotal int,
	waitingDeliveriesTotalItem int,
	waitingForPickUpTotal int,
	waitingForPickUpTotalItem int,
	notificationsTotal int
)

AS
BEGIN
		
		INSERT @StoreNotifSummary
		SELECT 
			   1 as Id,
			   PendingOrders.PendingOrdersTotal,
			   PendingOrders.PendingOrdersTotalItem,
			   PendingDeliveries.PendingDeliveriesTotal,
			   PendingDeliveries.PendingDeliveriesTotalItem,
			   PendingToReceive.PendingToReceiveReturnsTotal,
			   PendingToReceive.PendingToReceiveReturnsTotalItems,
			   PendingSales.PendingSalesTotal, 
			   PendingSales.PendingSalesTotalItem,
			   WaitingDel.WaitingDeliveriesTotal, 
			   WaitingDel.WaitingDeliveriesTotalItem,
			   WaitingForPickup.WaitingForPickUpTotal,
			   WaitingForPickup.WaitingForPickUpTotalItem,
			   0 as notificationsTotal

			FROM dbo.STGetPendingToReceiveReturnsNotif(@StoreId) AS PendingToReceive
				JOIN dbo.STGetPendingSalesNotif(@StoreId) AS PendingSales
					ON PendingToReceive.Id = PendingSales.Id

				JOIN dbo.GetPendingOrdersNotif(@StoreId) AS PendingOrders
					ON PendingToReceive.Id = PendingOrders.Id


				JOIN dbo.STGetPendingDeliveriesNotif(@StoreId) AS PendingDeliveries
					ON PendingToReceive.Id = PendingOrders.Id


				JOIN dbo.STGetWaitingDeliveriesNotif(@StoreId) AS WaitingDel
					ON PendingToReceive.Id = WaitingDel.Id

				JOIN dbo.STGetWaitingForPickUp(@StoreId) AS WaitingForPickup
					ON PendingToReceive.Id = WaitingForPickup.Id
	
    RETURN;

END
