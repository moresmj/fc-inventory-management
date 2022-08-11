using FC.Api.Validators.Store.AdvanceOrders;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Store.AdvanceOrders
{
    [Validator(typeof(ModifyAdvanceOrderDTOValidator))]
    public class ModifyAdvanceOrderDTO : STOrderDTO
    {


   
    }
}
