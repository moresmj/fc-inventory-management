ALTER VIEW ApproveTransferView
AS
	   SELECT id,
	   TransactionNo,
	   TransactionType,
	   RequestStatus,
	   PONumber,
	   PODate,
	   DeliveryType,
	   (SELECT CompanyId FROM Stores where id = StoreId) as OrderedBy,
	   (SELECT CompanyId FROM Stores where id = OrderToStoreId) as OrderedTo,
	   PaymentMode,
	   SalesAgent,
	   ClientName,
	   Address1 + ' ' + Address2 + ' ' + Address3 as Address ,
	   Remarks,
	   ORNumber,
	   SINumber,
	   WHDRNumber,
	   StoreId,
	   OrderToStoreId
	   FROM STOrders where OrderType = 3;
