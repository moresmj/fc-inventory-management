using FluentValidation.Attributes;
using InventorySystemAPI.Models.Store.Inventory;
using InventorySystemAPI.Validators.Request;

namespace InventorySystemAPI.Models.Request
{

    [Validator(typeof(PendingOrderValidator))]
    public class PendingOrder : STInventory
    { }
}
