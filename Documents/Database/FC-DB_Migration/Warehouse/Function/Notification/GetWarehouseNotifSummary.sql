CREATE FUNCTION [dbo].[GetWarehouseNotificationSumary](@WarehouseID int)
RETURNS @WarehouseNotifSummary TABLE
(
	Id int PRIMARY KEY NOT NULL, 
	WaitingDeliveriesTotal int,
	WaitingDeliveriesTotalItem int,
	WaitingForPickUpTotal int,
	WaitingForPickUpTotalItem int,
	NotificationsTotal int
)

AS
BEGIN
	DECLARE
		@WaitingDeliveriesTotal int,
		@WaitingDeliveriesTotalItem int,
		@WaitingForPickUpTotal int,
		@WaitingForPickUpTotalItem int,
		@NotificationsTotal int,
		@Id int;


	SET @WaitingDeliveriesTotal = (SELECT dbo.WHGetOrdersWaitingForDeliveryCount(@WarehouseId));
	SET @WaitingDeliveriesTotalItem = (SELECT dbo.WHGetTotalQtyForDelivery(@WarehouseId));
	SET @WaitingForPickUpTotal = (SELECT dbo.WHGetOrdersWaitingForPickupCount(@WarehouseId));
	SET @WaitingForPickUpTotalItem = (SELECT dbo.WHGetTotalQtyForPickUp(@WarehouseId));
	SET @NotificationsTotal = @WaitingDeliveriesTotal + @WaitingForPickUpTotal
	SET @Id = 1;

	IF @WaitingDeliveriesTotal IS NOT NULL 
    BEGIN
        INSERT @WarehouseNotifSummary
        SELECT @Id,@WaitingDeliveriesTotal, @WaitingDeliveriesTotalItem, @WaitingForPickUpTotal, @WaitingForPickUpTotalItem, @NotificationsTotal;
	END;
    RETURN;

END
