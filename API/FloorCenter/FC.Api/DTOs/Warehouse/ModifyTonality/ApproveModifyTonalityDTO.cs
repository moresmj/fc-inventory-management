using FC.Api.Validators.Warehouse.ModifyTonality;
using FC.Core.Domain.Common;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Warehouse.ModifyTonality
{
    [Validator(typeof(ApproveModifyTonalityDTOValidator))]
    public class ApproveModifyTonalityDTO
    {
        public int? Id { get; set; }

        public int? WarehouseId { get; set; }

        public int? STOrderId { get; set; }

        public RequestStatusEnum? RequestStatus { get; set; }

        public string Remarks { get; set; }

        public DateTime? DateApproved { get; set; }

        public DateTime? DateUpdated { get; set; }

        public ICollection<ApproveModifyTonalityDetailsDTO> ModifyItemTonalityDetails { get; set; }

    }
}
