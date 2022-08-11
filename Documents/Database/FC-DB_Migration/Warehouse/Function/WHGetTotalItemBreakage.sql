CREATE FUNCTION [dbo].[WHGetTotalItemBreakage](@WarehouseId INT, @ItemId INT) RETURNS INTEGER
AS
BEGIN

	DECLARE @Total as INT;
	
	SELECT @Total =
		CASE 
			WHEN SUM(OnHand) IS NULL THEN 0
			ELSE SUM(OnHand)
		END 
		FROM WHStocks 
	WHERE 
	WarehouseId = @WarehouseId and ItemId = @ItemId and DeliveryStatus = 1 and Broken = 1 and TransactionType = 1

	RETURN @Total;

END