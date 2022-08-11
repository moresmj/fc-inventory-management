using FluentValidation.Attributes;
using InventorySystemAPI.Validators.Store.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventorySystemAPI.Models.Store.Request
{
    [Validator(typeof(ForDeliveryUpdateValidator))]
    public class ForDeliveryUpdate : STDelivery
    { }
}
