-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		John Caranay
-- Create date:  04-24-2019
-- Description:	Warehouse Stock Summary Item Updater
-- =============================================
CREATE PROCEDURE SP_WarehouseStockSummaryUpdater
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Temporary Table for Items on warehouse that has changes
	DECLARE @ItemToBeUpdatedOnWarehouse TABLE (WarehouseId INT, ItemId INT);

	-- Temporary table for Warehouse Item Details - [Note: fields ae the same as the WarehouseStockSummary table because it will be used upon UPDATE or INSERT]
	DECLARE @ItemDetailsTempTable TABLE 
	(
		WarehouseId INT,
		ItemId INT,  
		ItemName VARCHAR(50),
		SerialNumber VARCHAR(50), 
		Code VARCHAR(50), 
		Tonality VARCHAR(50), 
		Description VARCHAR(255), 
		SizeId INT,
		SizeName VARCHAR(50),
		OnHand INT, 
		ForRelease INT, 
		Broken INT, 
		Available INT,
		Reserved INT
	)

	-- Used for storing the details upon Iteration of @ItemToBeUpdatedOnWarehouse
	DECLARE @WarehouseId INT, @ItemId INT;

	--DateTime offset 
	DECLARE @DateTimeOffset DateTime;
	SET @DateTimeOffset = (SELECT CONVERT(datetimeoffset, CURRENT_TIMESTAMP)   AT TIME ZONE 'China Standard Time'); 
	 

	-- Used on Time Filter
	DECLARE @currentTime DateTime = (SELECT CONVERT(datetime, @DateTimeOffset));
	DECLARE @timeFrom DateTime = DATEADD (MINUTE , -10 , @currentTime);

	IF ((SELECT COUNT(*) FROM WHStockSummary) = 0)
		BEGIN
			INSERT INTO @ItemToBeUpdatedOnWarehouse
			SELECT DISTINCT WarehouseId, ItemId FROM WHStocks;
		END
	ELSE
		BEGIN
			-- Store records on Temporary Table to be iterated later to fetch latest item inventory details
			INSERT INTO @ItemToBeUpdatedOnWarehouse
			SELECT WarehouseId, ItemId 
				FROM WHStocks 
			WHERE 
				(@timeFrom <= DateCreated and @currentTime >= DateCreated) or (@timeFrom <= DateUpdated and @currentTime >= DateUpdated) or (@timeFrom <= ChangeDate and @currentTime >= ChangeDate)
		END

	WHILE EXISTS (SELECT * FROM @ItemToBeUpdatedOnWarehouse)
	BEGIN
		-- GET the first record on the Temporary Table
		SELECT Top 1 @WarehouseId = WarehouseId, @ItemId = ItemId FROM @ItemToBeUpdatedOnWarehouse

		DECLARE @totalItemForReleasing INT = dbo.WHGetTotalItemForReleasing(@WarehouseId, @ItemId);
		DECLARE @totalItemAvailable INT = dbo.WHGetTotalItemAvailable(@WarehouseId, @ItemId);
		DECLARE @totalItemReceivedBroken INT = dbo.WHGetTotalItemReceivedBroken(@WarehouseId, @ItemId);
		DECLARE @totalItemBreakage INT = dbo.WHGetTotalItemBreakage(@WarehouseId, @ItemId);
		--Added for advance order reserved
		DECLARE @totalReserved INT = dbo.WHGetReserved(@WarehouseId, @ItemId);
		SET @totalItemAvailable = @totalItemAvailable + @totalItemBreakage;

		-- Fetch and insert item inventory details on @ItemDetailsTempTable
		INSERT INTO @ItemDetailsTempTable
			SELECT	 @WarehouseId, @ItemId,
						I.Name, I.SerialNumber, I.Code, I.Tonality, I.Description,
						S.Id, S.Name,	 
						(@totalItemAvailable + @totalItemReceivedBroken + @totalReserved) as OnHand,
							@totalItemForReleasing as ForRelease,
						(@totalItemReceivedBroken + (@totalItemBreakage * -1)) as Broken,
						(@totalItemAvailable - @totalItemForReleasing) as Available,
						 @totalReserved as Reserved
			FROM Items as I LEFT JOIN Sizes as S ON I.SizeId = S.Id  
			WHERE I.Id = @ItemId

		-- Remove the record selected on @ItemToBeUpdatedOnWarehouse
		DELETE FROM @ItemToBeUpdatedOnWarehouse WHERE WarehouseId = @WarehouseId and ItemId = @ItemId;
	END

	-- Details :
	-- T1 - Table to be Updated or Inserted
	-- T2 - Temporary table to be referenced for the UPDATE or INSERT of new record to T1
	-- NOTE : The on clause determine if an UPDATE or INSERT should occur.
	MERGE dbo.WHStockSummary T1
	USING (
			SELECT * FROM @ItemDetailsTempTable
			) T2
		ON T1.WarehouseId = T2.WarehouseId and T1.ItemId = T2.ItemId
	WHEN MATCHED THEN
		UPDATE 
		SET 
			T1.ItemName = T2.ItemName,
			T1.Code = T2.Code,
			T1.Tonality = T2.Tonality,
			T1.Description = T2.Description,
			T1.SizeId = T2.SizeId,
			T1.SizeName = T2.SizeName,
			T1.OnHand = T2.OnHand,
			T1.ForRelease = T2.ForRelease,
			T1.Broken = T2.Broken,
			T1.Available = T2.Available,
			T1.Reserved = T2.Reserved,
			T1.DateUpdated = CURRENT_TIMESTAMP
	WHEN NOT MATCHED THEN
		INSERT (WarehouseId, ItemId, ItemName, SerialNumber, Code, Tonality, Description, SizeId, SizeName, OnHand, ForRelease, Broken, Available,Reserved, DateCreated)
		VALUES (T2.WarehouseId, T2.ItemId, T2.ItemName, T2.SerialNumber, T2.Code, T2.Tonality, T2.Description, T2.SizeId, T2.SizeName, T2.OnHand, T2.ForRelease, T2.Broken, T2.Available, T2.Reserved, (SELECT CURRENT_TIMESTAMP));
END
GO