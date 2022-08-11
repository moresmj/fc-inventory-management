CREATE FUNCTION [dbo].[STGetItemBrokenRegisterStore](@StoreId int, @ItemId int) RETURNS INTEGER
AS
BEGIN

	DECLARE @TOTAL as INT;
	
	
	SELECT @TOTAL = 
		CASE 
			WHEN SUM(OnHand) IS NULL THEN 0
			ELSE SUM(OnHand)
			END
	FROM STStocks 
	WHERE 
		StoreId = @StoreId 
		and ItemId = @ItemId 
		and DeliveryStatus = 1 
		and Broken = 1 
		and STImportDetailId IS NOT NULL

	RETURN @Total;

END
