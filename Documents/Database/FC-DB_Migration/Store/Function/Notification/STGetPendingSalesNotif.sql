CREATE FUNCTION [dbo].[STGetPendingSalesNotif](@StoreId INT)
RETURNS @PendingSales TABLE
(
	Id INT,
	PendingSalesTotal INT,
	PendingSalesTotalItem INT
)
AS
	BEGIN
		INSERT @PendingSales
		SELECT 1 as Id,
			 	COUNT(DISTINCT Sales.Id) AS PendingSalesTotal, 
				SUM(Details.Quantity) AS PendingSalesTotalItem 
			FROM STSales AS Sales
			LEFT JOIN STSalesDetails AS Details
			ON Sales.Id = Details.STSalesId
			WHERE Sales.StoreId = @StoreId AND Sales.SalesType = 3 AND Sales.ORNumber IS NULL

		RETURN;

	END