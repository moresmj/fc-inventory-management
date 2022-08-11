using FC.Api.Validators.Item;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Item
{
    [Validator(typeof(ItemWIthImageDTOValidator))]
    public class ItemWithImageDTO : ItemDTO
    {

    }
}
