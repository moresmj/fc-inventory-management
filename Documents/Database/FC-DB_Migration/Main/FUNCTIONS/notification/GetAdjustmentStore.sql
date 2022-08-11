CREATE FUNCTION [dbo].[GetAdjustmentStore]()
RETURNS @AdjustmentRequestSummary TABLE
(
	StoreAdjustmentTotal int,
	StoreAdjustmentTotalItem int
)
AS
BEGIN
	INSERT @AdjustmentRequestSummary
		SELECT COUNT(*) as StoreAdjustmentTotal,
		COALESCE(SUM(ABS(STImportDetails.SystemCount - STImportDetails.PhysicalCount)),0) as PendingReturnsTotalItem
		FROM STImports LEFT JOIN STImportDetails
		ON STImportDetails.STImportId = STImports.Id
		WHERE RequestStatus = 2;
	RETURN;

END