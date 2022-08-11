CREATE FUNCTION [dbo].[STGetItemAvailableQuantityStore](@StoreId int, @ItemId int) RETURNS INTEGER
AS
BEGIN

	DECLARE @TOTAL as INT;
	
	
	SELECT @Total = 
		CASE 
			WHEN SUM(OnHand) IS NULL THEN 0
			ELSE SUM(OnHand)
			END
	FROM STStocks 
	WHERE 
		StoreId = @StoreId and ItemId = @ItemId and
		(DeliveryStatus = 1 or ReleaseStatus = 1) and
		Broken = 0 

	RETURN @Total;

END