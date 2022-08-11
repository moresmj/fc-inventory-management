CREATE FUNCTION [dbo].[FSIsInterbranch](@StoreId INT, @OrderToStoreId INT) RETURNS BIT
AS
BEGIN
	--Well check if the transaction is interbranch
	DECLARE @OrderedBy as INT;
	DECLARE @OrderedTo as INT;

	SET @OrderedBy = (SELECT CompanyId FROM Stores where Id = @StoreId);

	SET @OrderedTo = (SELECT CompanyId FROM Stores where Id = @OrderToStoreId);


	IF(@OrderedBy = @OrderedTo)
		BEGIN
			RETURN 1;
		END	

	 RETURN 0;

END