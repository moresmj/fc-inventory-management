CREATE FUNCTION [dbo].[GetApproveRequestSummary]()
RETURNS @ForApprovalRequestSummary TABLE
(
	ApproveRequestTotal int,
	ApproveRequestItemTotal int

)

AS 
BEGIN


		INSERT @ForApprovalRequestSummary
			  SELECT  COUNT(DISTINCT STOrders.Id), SUM(RequestedQuantity) FROM  STOrders 
			  LEFT JOIN STOrderDetails ON
			  STOrders.Id = STOrderDetails.STOrderId
			  WHERE STOrders.OrderType != 3 AND STOrders.RequestStatus = 2;

		RETURN;
END


