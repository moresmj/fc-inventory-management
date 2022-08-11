using FC.Api.DTOs.Warehouse.ModifyTonality;
using FC.Api.Helpers;
using FC.Core.Domain.Warehouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.Warehouses.ModifyTonality
{
    public interface IModifyTonalityService
    {
        DataContext DataContext();

        object GetAllForApprovalRequests_ChangeItemTonality(SearchModifyTonality search, AppSettings appSettings);

        void ApproveModifyTonality(ApproveModifyTonalityDTO param);
    }
}
