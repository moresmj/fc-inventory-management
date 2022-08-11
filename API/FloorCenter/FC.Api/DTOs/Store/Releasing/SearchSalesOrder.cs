using FC.Core.Domain.Common;

namespace FC.Api.DTOs.Store.Releasing
{
    public class SearchSalesOrder : BaseSearch
    {
        public ReleaseStatusEnum?[] ReleaseStatus { get; set; }
    }
}
