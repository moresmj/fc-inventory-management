using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Items;
using FC.Core.Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace FC.Api.Controllers
{
    [Produces("application/json")]
    [Route("api")]
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
        [Route("purposes1/all")]
        public IActionResult GetPurposes1()
        {
            return Ok(EnumExtensions.GetValues<Purpose1Enum>());
        }

        [HttpGet]
        [Route("purposes2/all")]
        public IActionResult GetPurposes2()
        {
            return Ok(EnumExtensions.GetValues<Purpose2Enum>());
        }

        [HttpGet]
        [Route("traffics/all")]
        public IActionResult GetTraffics()
        {
            return Ok(EnumExtensions.GetValues<TrafficEnum>());
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
        [Route("releasestatus/all")]
        public IActionResult GetReleaseStatus()
        {
            return Ok(EnumExtensions.GetValues<ReleaseStatusEnum>());
        }

        [HttpGet]
        [Route("ordertypes/all")]
        public IActionResult GetOrderTypes()
        {
            return Ok(EnumExtensions.GetValues<OrderTypeEnum>());
        }

        [HttpGet]
        [Route("deliverytypes/all")]
        public IActionResult GetDeliveryTypees()
        {
            return Ok(EnumExtensions.GetValues<DeliveryTypeEnum>());
        }

        [HttpGet]
        [Route("preferredtime/all")]
        public IActionResult GetPreferredTime()
        {
            return Ok(EnumExtensions.GetValues<PreferredTimeEnum>());
        }

        [HttpGet]
        [Route("returnreason/all")]
        public IActionResult GetReturnReason()
        {
            return Ok(EnumExtensions.GetValues<ReturnReasonEnum>());
        }

        [HttpGet]
        [Route("returntype/all")]
        public IActionResult GetReturnType()
        {
            return Ok(EnumExtensions.GetValues<ReturnTypeEnum>());
        }

        [HttpGet]
        [Route("clientreturntype/all")]
        public IActionResult GetClientReturnType()
        {
            return Ok(EnumExtensions.GetValues<ClientReturnTypeEnum>());
        }

        [HttpGet]
        [Route("paymentmodes/all")]
        public IActionResult GetPaymentModes()
        {
            return Ok(EnumExtensions.GetValues<PaymentModeEnum>());
        }

        [HttpGet]
        [Route("storecompanyrelation/all")]
        public IActionResult GetStoreCompanyRelations()
        {
            return Ok(EnumExtensions.GetValues<StoreCompanyRelationEnum>());
        }


        [HttpGet]
        [Route("orderstatus/all")]
        public IActionResult GetOrderStatus()
        {
            return Ok(EnumExtensions.GetValues<OrderStatusEnum>());
        }

        [HttpGet]
        [Route("itemtypes")]
        public IActionResult GetItemTypes()
        {
            return Ok(EnumExtensions.GetValues<ItemTypeEnum>());
        }

    }

}