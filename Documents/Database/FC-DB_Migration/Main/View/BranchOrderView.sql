-- ================================================
-- Getting Branch order view for branch orders
-- ================================================
CREATE VIEW BranchOrderView
AS
	SELECT Id,
	TransactionNo,
	TransactionType,
	PONumber,
	PODate,
	DeliveryType,
	StoreId,
	(SELECT Name from Stores where Id = StoreId) as OrderedBy,
	(SELECT Name from Stores where Id = OrderToStoreId) as OrderedTo,
	PaymentMode,
	SalesAgent,
	ClientName,
	Address1,
	Address2,
	Address3,
	Remarks,
	ORNumber,
	SINumber,
	WHDRNumber,
	(SELECT dbo.FSIsInterbranch(StoreId,OrderToStoreId)) as IsInterbranch,
	OrderToStoreId
	FROM STOrders 
	WHERE OrderType = 3 AND RequestStatus = 1

