CREATE FUNCTION [dbo].[STGetTotalItemForReleasing](@StoreId int, @ItemId int) RETURNS INTEGER
AS
BEGIN

	
	DECLARE @totalForReleasing INT;
	DECLARE @ForReleasingTotal INT;
	DECLARE @NotForReleasing INT;
	DECLARE @ForReleasing INT;
	DECLARE @SoldQty INT;
	DECLARE @DeliveredQty INT;
	DECLARE @DeliveredTransferQty INT = 0;
	DECLARE @OrderTransferQty INT = 0;

	-- CREATED TEMP TABLE FOR STORING  ORDERS RELEASED QUANTITY 
	DECLARE @ForReleaseTempTable TABLE ([onHand] int, [isNotForRelease] bit);


	INSERT INTO @ForReleaseTempTable (onHand, isNotForRelease) 
	SELECT stStocks.OnHand,
	(SELECT 
		CASE 
			WHEN StoreCompanyFrom.CompanyId = StoreCompanyTo.CompanyId 
				THEN  (CASE WHEN stOrders.ORNumber IS NULL THEN 1 ELSE 0 END) 

				ELSE (CASE WHEN stOrders.SINumber IS NULL AND stOrders.WHDRNumber IS NULL THEN 1 ELSE 0 END) END) AS isNotForRelease

			From STStocks as stStocks 
			JOIN STOrderDetails as storderDetails 
			ON stStocks.STOrderDetailId = storderDetails.Id 
			JOIN STOrders as stOrders
			ON stOrders.Id = storderDetails.STOrderId
			JOIN Stores as StoreCompanyFrom
			on stOrders.StoreId = StoreCompanyFrom.Id
			JOIN Stores as StoreCompanyTo
			on stOrders.OrderToStoreId = StoreCompanyTo.Id
			WHERE  stStocks.StoreId = @StoreId 
				AND stStocks.ItemId = @ItemId
				AND stStocks.STOrderDetailId IS NOT NULL
				AND ( stStocks.DeliveryStatus = 2 OR stStocks.DeliveryStatus = 3) 
				AND ( stStocks.ReleaseStatus = 3 OR stStocks.ReleaseStatus = 2)
				AND (stOrders.RequestStatus = 1  AND storders.OrderType = 3)

		SET @NotForReleasing = (SELECT  COALESCE(SUM(onHand), 0)  as onHand FROM @ForReleaseTempTable WHERE isNotForRelease = 1);
	


		SET @ForReleasing =( SELECT ABS(SUM(STOCKS.OnHand))
					FROM [STStocks] AS STOCKS
					LEFT JOIN [STOrderDetails] AS STOrderDetail ON STOCKS.[STOrderDetailId] =STOrderDetail.[Id]
					WHERE 
					((((STOCKS.[StoreId] = @StoreId) 
					AND (STOCKS.[ItemId] = @ItemId)) 
					AND STOCKS.[STOrderDetailId] IS NOT NULL) 
					AND ((STOCKS.[DeliveryStatus] = 3) OR (STOCKS.[DeliveryStatus] = 2))) 
					AND ((STOCKS.[ReleaseStatus] = 2) OR (STOCKS.[ReleaseStatus] = 3))
					)


				


		SET @ForReleasingTotal =(CASE 
									WHEN @ForReleasing IS NULL THEN @NotForReleasing
										ELSE @ForReleasing + @NotForReleasing
								END);


	


		SET @SoldQty = (SELECT COALESCE
									((SELECT SUM(salesDetails.Quantity) as soldQty FROM STSales as Sales
											 JOIN STSalesDetails as salesDetails ON Sales.Id = salesDetails.STSalesId
											 WHERE Sales.StoreId = @StoreId
											 AND Sales.SalesType = 1 
											 AND (Sales.DeliveryType = 2 OR Sales.DeliveryType = 1)
											 AND salesDetails.ItemId = @ItemId
											 GROUP BY salesDetails.ItemId), 0
									 )
						);

	
		--Set sales temporary table
		DECLARE @SalesTempTable TABLE (SALESid int ,SALESStoreId int, SalesSalesType int,clientDeliveriesItemId int,clientDeliveriesQuantity int);
		INSERT INTO @SalesTempTable (SALESid  ,SALESStoreId , SalesSalesType ,clientDeliveriesItemId ,clientDeliveriesQuantity) 
        --Removed distinct query to count records with multiple deliveries
		SELECT SALES.id ,SALES.StoreId, Sales.SalesType,clientDeliveries.ItemId,clientDeliveries.Quantity 
		FROM STSales as Sales
		JOIN STSalesDetails as salesDetails ON Sales.Id = salesDetails.STSalesId
		JOIN STDeliveries as deliveries ON sales.Id = deliveries.STSalesId
		JOIN STClientDeliveries as clientDeliveries ON deliveries.Id = clientDeliveries.STDeliveryId
		WHERE (
				Sales.StoreId = @StoreId
				AND Sales.SalesType = 1 
				AND clientDeliveries.ItemId = @ItemId
				AND clientDeliveries.DeliveryStatus = 1 
				AND clientDeliveries.ReleaseStatus = 1
				AND (Sales.DeliveryType = 2 OR Sales.DeliveryType = 1)
			  )

		-- If query returned null will replace it to 0
		SET @DeliveredQty = (SELECT COALESCE 
								((SELECT SUM(clientDeliveriesQuantity) FROM @SalesTempTable), 0)
							);


	
		--GetTransferRecordsStore
		--DECLARE @TransferTempTable TABLE ([ShowroomDelQty] int,[ClientDelQty] int,[STCDItemId] int,[STCDReleaseStatus] int,[STSDItemId] int,[STSDReleaseStatus] int,[STODItemID] int, [STODReleaseStatus] int, [STODApprovedQty] int, [STODSTOrderId] int, [isForRelease] bit);

		DECLARE @STORId INT
		DECLARE @isForRelease BIT
		DECLARE @TransferTempTable TABLE(Id INT, isForRelease BIT);

		INSERT INTO @TransferTempTable([Id], [isForRelease])
		SELECT STORD.Id,
		(SELECT 
			CASE 
				WHEN StoreCompanyFrom.CompanyId = StoreCompanyTo.CompanyId 
					THEN  (CASE WHEN STORD.ORNumber IS NOT NULL THEN 1 ELSE 0 END) 

					ELSE (CASE WHEN STORD.SINumber IS NOT NULL AND STORD.WHDRNumber IS NOT NULL THEN 1 ELSE 0 END) 
					
			END) AS isForRelease
		FROM STOrders as STORD
		LEFT JOIN STOrderDetails as STORDet ON STORD.Id = STORDet.STOrderId
		LEFT JOIN Stores as StoreCompanyFrom ON STORD.StoreId = StoreCompanyFrom.Id
		LEFT JOIN Stores as StoreCompanyTo ON STORD.OrderToStoreId = StoreCompanyTo.Id
		WHERE (STORD.OrderToStoreId = @StoreId
		AND STORD.OrderType = 3
		AND STORD.DeliveryType <> 1
		AND STORDet.ReleaseStatus <> 1)



		WHILE EXISTS (SELECT * FROM @TransferTempTable)
			BEGIN
				SELECT TOP 1 @STORId = Id, @isForRelease = isForRelease FROM @TransferTempTable;

				if(@isForRelease = 1)
					BEGIN
						SET @DeliveredTransferQty += (SELECT COALESCE(SUM(STClientDel.Quantity),0) FROM STDeliveries as STDel
							LEFT JOIN STClientDeliveries as STClientDel on STDel.Id = STClientDel.STDeliveryId
							WHERE (STDel.STOrderId = @STORId
							AND STClientDel.ItemId = @ItemId
							AND STClientDel.ReleaseStatus = 1));

						SET @DeliveredTransferQty += (SELECT COALESCE(SUM(STShowDel.Quantity),0) FROM STDeliveries as STDel
							LEFT JOIN STShowroomDeliveries as STShowDel on STDel.Id = STShowDel.STDeliveryId
							WHERE (STDel.STOrderId = @STORId
							AND STShowDel.ItemId = @ItemId
							AND STShowDel.ReleaseStatus = 1));

						SET @OrderTransferQty += (SELECT  COALESCE(SUM(ApprovedQuantity),0)  FROM STOrderDetails
												 WHERE ItemId = @ItemId AND ReleaseStatus = 3 AND STOrderId = @STORId);
					END

				DELETE FROM @TransferTempTable WHERE id = @STORId
			END
		--GetTransferRecordsStore end



		--GET TOTAL WAREHOUSE DELIVERY
		DECLARE @ReturnsTempTable TABLE (Id INT, ReturnType INT);
		DECLARE @Id INT, @ReturnType INT  = 0;
		DECLARE @totalWarehouseDeliveries INT = 0;
		
		DECLARE @totalReturnQuantity INT  = 0;


		INSERT INTO @ReturnsTempTable
		SELECT id, ReturnType FROM STReturns as STRe 
			WHERE (StoreId = @StoreId and ((ReturnType = 1 and RequestStatus = 1) or (ReturnType = 3 and RequestStatus = 2)));
				
		WHILE EXISTS (SELECT * FROM @ReturnsTempTable)
		BEGIN
			DECLARE @totalReleasedItem INT  = 0;
			SELECT TOP 1 @Id = id, @ReturnType = ReturnType FROM @ReturnsTempTable;
	
			IF ((SELECT COUNT(*) FROM WHDeliveries WHERE STReturnId = @Id) > 0 and @ReturnType <> 3)
			BEGIN
				IF(@ReturnType <> 1)
				BEGIN
					SET @totalWarehouseDeliveries = (SELECT SUM(WHDelDetails.Quantity)
															FROM WHDeliveries as WHDel LEFT JOIN WHDeliveryDetails as WHDelDetails ON WHDel.Id = WHDelDetails.WHDeliveryId
															WHERE WHDelDetails.DeliveryStatus = 3
															AND WHDelDetails.ReleaseStatus = 3
															AND WHDelDetails.ItemId = @ItemId)
				END

				SET @totalReleasedItem = (SELECT COALESCE(SUM(WHDELDet.Quantity),0) FROM WHDeliveries as WHDEL
							LEFT JOIN WHDeliveryDetails as WHDELDet on WHDEL.Id = WHDELDet.WHDeliveryId
							LEFT JOIN STReturns as STRT on STRT.id = WHDEL.STReturnId
							WHERE ((WHDELDet.DeliveryStatus = 1  OR WHDELDet.DeliveryStatus = 3)
							AND WHDELDet.ReleaseStatus = 1
							AND WHDELDet.ItemId = @ItemId
							AND WHDEL.StoreId = @StoreId
							AND WHDEL.STReturnId = @Id))



					

			END
			IF ( (SELECT COUNT(*) FROM STPurchaseReturns WHERE STReturnId = @Id) > 0 )
			BEGIN
				DECLARE @totalGoodQty INT = 0; 

				SET @totalGoodQty = (SELECT COALESCE(SUM(GoodQuantity),0) +  COALESCE(SUM(BrokenQuantity),0)
									FROM STPurchaseReturns 
									WHERE 
										(STReturnId = @Id and DeliveryStatus = 3 and ReleaseStatus = 3 and ItemId = @ItemId));
				IF (@ReturnType = 3)	
				BEGIN
					SET @totalGoodQty += (SELECT COALESCE(SUM(BrokenQuantity),0) 
									FROM STPurchaseReturns 
									WHERE 
										((STReturnId = @Id and DeliveryStatus = 2 AND ReleaseStatus = 2) OR DeliveryStatus = 5) AND ItemId = @ItemId);

					SET @totalReleasedItem += (SELECT COALESCE(SUM(WHDELDet.Quantity),0) FROM WHDeliveries as WHDEL
							LEFT JOIN WHDeliveryDetails as WHDELDet on WHDEL.Id = WHDELDet.WHDeliveryId
							LEFT JOIN STReturns as STRT on STRT.id = WHDEL.STReturnId
							WHERE ((WHDELDet.DeliveryStatus = 1  OR WHDELDet.DeliveryStatus = 3)
							AND WHDELDet.ReleaseStatus = 1
							AND WHDELDet.ItemId = @ItemId
							AND WHDEL.StoreId = @StoreId
							AND WHDEL.STReturnId = @Id));
				END	
								
					SET @totalReturnQuantity += @totalGoodQty;

					IF(@totalGoodQty <> 0)
					BEGIN
						SET @totalReturnQuantity = @totalReturnQuantity - @totalReleasedItem;
					END

			END

			DELETE FROM @ReturnsTempTable WHERE id = @Id

		END


			SET @totalWarehouseDeliveries = @totalWarehouseDeliveries + @totalReturnQuantity;

			IF(@totalWarehouseDeliveries IS NULL)
				BEGIN
					SET @totalWarehouseDeliveries = 0
				END

		--GET TOTAL WAREHOUSE DELIVERY END

		SET @totalForReleasing = @ForReleasingTotal + (@SoldQty - @DeliveredQty) + (@OrderTransferQty - @DeliveredTransferQty) + @totalWarehouseDeliveries;

			
		
		RETURN @totalForReleasing

END