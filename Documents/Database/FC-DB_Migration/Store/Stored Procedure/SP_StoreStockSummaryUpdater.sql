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
-- Author:		JERINIEL FERNANDEZ
-- Create date:  04-25-2019
-- Description:	Store Stock Summary Item Updater
-- =============================================
CREATE PROCEDURE SP_StoreStockSummaryUpdater
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Temporary Table for Items on Store that has changes
	DECLARE @ItemToBeUpdatedOnStore TABLE (StoreId INT, ItemId INT);

	-- Temporary table for Store Item Details - [Note: fields ae the same as the StoreStockSummary table because it will be used upon UPDATE or INSERT]
	DECLARE @ItemDetailsTempTable TABLE 
	(
		StoreId INT,
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
		Available INT
	)

	-- Used for storing the details upon Iteration of @ItemToBeUpdatedOnStore
	DECLARE @StoreId INT, @ItemId INT;
	
	--DateTime offset 
	 DECLARE @DateTimeOffset DateTime;
	 SET @DateTimeOffset = (SELECT CONVERT(datetimeoffset, CURRENT_TIMESTAMP)   AT TIME ZONE 'China Standard Time'); 
	 

	-- Used on Time Filter
	DECLARE @currentTime DateTime = (SELECT CONVERT(datetime, @DateTimeOffset));
	DECLARE @timeFrom DateTime = DATEADD (MINUTE , -10 , @currentTime);

	IF ((SELECT COUNT(*) FROM STStockSummary) = 0)
		BEGIN
			INSERT INTO @ItemToBeUpdatedOnStore
			SELECT DISTINCT StoreId, ItemId FROM STStocks;
		END
	ELSE
		BEGIN
			-- Store records on Temporary Table to be iterated later to fetch latest item inventory details
			INSERT INTO @ItemToBeUpdatedOnStore
			SELECT StoreId, ItemId 
				FROM STStocks 
			WHERE 
				(@timeFrom <= DateCreated and @currentTime >= DateCreated) or (@timeFrom <= DateUpdated and @currentTime >= DateUpdated) or (@timeFrom <= ChangeDate and @currentTime >= ChangeDate)
		END

	WHILE EXISTS (SELECT * FROM @ItemToBeUpdatedOnStore)
	BEGIN
		-- GET the first record on the Temporary Table
		SELECT Top 1 @StoreId = StoreId, @ItemId = ItemId FROM @ItemToBeUpdatedOnStore

		DECLARE @totalItemForReleasing INT = dbo.STGetTotalItemForReleasing(@StoreId, @ItemId);
		DECLARE @totalItemAvailable INT = dbo.STGetItemAvailableQuantityStore(@StoreId,@ItemId) - @totalItemForReleasing;
		DECLARE @totalItemBroken INT = dbo.STGetItemBrokenRegisterStore(@StoreId,@ItemId)
		DECLARE @totalItemReceivedBroken INT = dbo.STGetItemBrokenQuantityStore(@StoreId, @ItemId);
		DECLARE @totalBreakage INT = @totalItemBroken + @totalItemReceivedBroken;

		-- Fetch and insert item inventory details on @ItemDetailsTempTable
		INSERT INTO @ItemDetailsTempTable
			SELECT	 @StoreId, @ItemId,
						I.Name, I.SerialNumber, I.Code, I.Tonality, I.Description,
						S.Id, S.Name,	 
						(@totalItemAvailable + @totalBreakage + @totalItemForReleasing) as OnHand,
						 @totalItemForReleasing as ForRelease,
						 @totalBreakage as Broken,
						(@totalItemAvailable) as Available
			FROM Items as I LEFT JOIN Sizes as S ON I.SizeId = S.Id  
			WHERE I.Id = @ItemId

		-- Remove the record selected on @ItemToBeUpdatedOnStore
		DELETE FROM @ItemToBeUpdatedOnStore WHERE StoreId = @StoreId and ItemId = @ItemId;
	END

	-- Details :
	-- T1 - Table to be Updated or Inserted
	-- T2 - Temporary table to be referenced for the UPDATE or INSERT of new record to T1
	-- NOTE : The on clause determine if an UPDATE or INSERT should occur.
	MERGE dbo.STStockSummary T1
	USING (
			SELECT * FROM @ItemDetailsTempTable
			) T2
		ON T1.StoreId = T2.StoreId and T1.ItemId = T2.ItemId
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
			T1.DateUpdated = CURRENT_TIMESTAMP
	WHEN NOT MATCHED THEN
		INSERT (StoreId, ItemId, ItemName, SerialNumber, Code, Tonality, Description, SizeId, SizeName, OnHand, ForRelease, Broken, Available, DateCreated)
		VALUES (T2.StoreId, T2.ItemId, T2.ItemName, T2.SerialNumber, T2.Code, T2.Tonality, T2.Description, T2.SizeId, T2.SizeName, T2.OnHand, T2.ForRelease, T2.Broken, T2.Available, (SELECT CURRENT_TIMESTAMP));
END
GO
