CREATE FUNCTION [dbo].[FSWHGetOnhand](@WarehouseId INT, @ItemId INT,@DeductForRelease BIT) RETURNS INTEGER
AS
BEGIN
	--Former GetItemAvailable on WHStockService
	--getting total onhand of item
	DECLARE @OnHand as INT;
	DECLARE @ForReleasing as INT;
	DECLARE @TotalBreakage as INT;

	SET @OnHand =  (SELECT dbo.WHGetTotalItemAvailable(@WarehouseId, @ItemId));
	--moved because of new modification item will be for releasing once order is added
	SET @TotalBreakage = (SELECT dbo.WHGetTotalItemBreakage(@WarehouseId, @ItemId));
	SET @OnHand = @OnHand + @TotalBreakage;
	
	--if true will deduct for releasing
	IF(@DeductForRelease = 1)
		BEGIN
			SET @ForReleasing = (SELECT dbo.WHGetTotalItemForReleasing(@WarehouseId, @ItemId));
			--remove because of new modification item will be for releasing once order is added
			--SET @TotalBreakage = (SELECT dbo.WHGetTotalItemBreakage(@WarehouseId, @ItemId));
			--SET @OnHand = @OnHand + @TotalBreakage;
			SET @OnHand = @OnHand - @ForReleasing;
		END	

	RETURN @OnHand;

END