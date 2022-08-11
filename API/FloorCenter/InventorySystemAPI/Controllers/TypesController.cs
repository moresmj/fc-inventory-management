using InventorySystemAPI.Helpers;
using InventorySystemAPI.Models.Store.Inventory;
using InventorySystemAPI.Models.Store.Sales;
using InventorySystemAPI.Models.User;
using InventorySystemAPI.Models.Warehouse.Stock;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystemAPI.Controllers
{

    [Route("api/")]
    public class TypesController : Controller
    {


        [HttpGet]
        [Route("usertypes/all")]
        public IActionResult GetUserTypes()
        {
            return Ok(EnumExtensions.GetValues<UserTypeEnum>());
        }

        [HttpGet]
        [Route("assignments/all")]
        public IActionResult GetAssignments()
        {
            return Ok(EnumExtensions.GetValues<AssignmentEnum>());
        }

        [HttpGet]
        [Route("deliverystatus/all")]
        public IActionResult GetDeliveryStatus()
        {
            return Ok(EnumExtensions.GetValues<DeliveryStatusEnum>());
        }

        [HttpGet]
        [Route("transactiontypes/all")]
        public IActionResult GetTransactionTypes()
        {
            return Ok(EnumExtensions.GetValues<TransactionTypeEnum>());
        }

        [HttpGet]
        [Route("requeststatus/all")]
        public IActionResult GetRequestStatus()
        {
            return Ok(EnumExtensions.GetValues<RequestStatusEnum>());
        }

        [HttpGet]
        [Route("paymenttypes/all")]
        public IActionResult GetPaymentTypes()
        {
            return Ok(EnumExtensions.GetValues<PaymentTypeEnum>());
        }

        [HttpGet]
        [Route("deliverytypes/all")]
        public IActionResult GetDeliveryTypes()
        {
            return Ok(EnumExtensions.GetValues<DeliveryTypeEnum>());
        }
    }

}