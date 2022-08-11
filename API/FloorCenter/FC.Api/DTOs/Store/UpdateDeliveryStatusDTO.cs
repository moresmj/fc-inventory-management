using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Store
{
    [Validator(typeof(UpdateDeliveryStatusDTOValidator))]
    public class UpdateDeliveryStatusDTO : STDeliveryDTO
    {
        public bool IsDelivered { get; set; }
    }

}
