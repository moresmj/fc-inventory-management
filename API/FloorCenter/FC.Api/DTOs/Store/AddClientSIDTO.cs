using FC.Api.Validators.Store;
using FC.Core;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Store
{
    [Validator(typeof(AddClientSIValidator))]
    public class AddClientSIDTO : BaseEntity
    {


        public string ClientSINumber { get; set; }
    }
}
