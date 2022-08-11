CREATE FUNCTION [dbo].[FSSTGetOnhand](@StoreId INT, @ItemId INT,@DeductForRelease BIT) RETURNS INTEGER
AS
BEGIN
	--Former GetItemAvailableQuantity on StStockService
	--getting total onhand of item
	DECLARE @OnHand as INT;
	DECLARE @ForReleasing as INT = 0;

	SET @OnHand =  (SELECT dbo.STGetItemAvailableQuantityStore(@StoreId, @ItemId));
	
	--if true will deduct for releasing
	IF(@DeductForRelease = 1)
		BEGIN
			SET @ForReleasing = (SELECT dbo.STGetTotalItemForReleasing(@StoreId, @ItemId));
		END	

	RETURN @OnHand - @ForReleasing;;

END