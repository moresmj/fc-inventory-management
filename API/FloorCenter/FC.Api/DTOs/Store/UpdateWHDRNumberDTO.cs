using FC.Api.Validators;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Store
{
    [Validator(typeof(UpdateWHDRNumberDTOValidator))]
    public class UpdateWHDRNumberDTO : STOrderDTO
    { }
}
