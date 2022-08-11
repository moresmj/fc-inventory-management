CREATE FUNCTION [dbo].[GetReturnPendingSummary]()
RETURNS @ReturnPendingSummary TABLE
(
  NotificationsTotal int,
  PendingReturnsTotal int,
  PendingReturnsTotalItem int
)

AS

BEGIN

	DECLARE
		@NotificationsTotal int,
		@PendingReturnsTotal int,
		@PendingReturnsTotalItem int;
	--Will create table to insert the filtered records
	DECLARE @ForReturnTempTable TABLE ([Id] int,[TotalQty] int,[TotalQtyBroken] int, [DeliveryStatus] int, [ReturnType] int);

	--Insert filtered records on the temp table
	INSERT INTO @ForReturnTempTable
	SELECT 
		STReturns.Id,
		STPurchaseReturns.BrokenQuantity,
		STPurchaseReturns.GoodQuantity,
		STPurchaseReturns.DeliveryStatus,
		STReturns.ReturnType 
	FROM STReturns
		JOIN STPurchaseReturns ON STReturns.Id = STPurchaseReturns.STReturnId 
	Where ReturnType != 2 AND RequestStatus = 2;




	

	--Get total from temp table assign to variables
	SET @NotificationsTotal =  (SELECT  COUNT(DISTINCT ID) FROM  @ForReturnTempTable WHERE (ReturnType = 3 and DeliveryStatus = 1) OR ReturnType != 3);
	SET @PendingReturnsTotal =  (SELECT  COUNT(DISTINCT ID) FROM  @ForReturnTempTable WHERE (ReturnType = 3 and DeliveryStatus = 1) OR ReturnType != 3);
	SET @PendingReturnsTotalItem = (SELECT SUM(TotalQty + TotalQtyBroken) FROM @ForReturnTempTable WHERE (ReturnType = 3 and DeliveryStatus = 1) OR ReturnType != 3)

	--Insert and return records
	IF @PendingReturnsTotalItem IS NOT NULL 
    BEGIN
        INSERT @ReturnPendingSummary
        SELECT @NotificationsTotal,@PendingReturnsTotal, @PendingReturnsTotalItem
	END;
    RETURN;


END

