using AutoMapper;
using FC.Api.DTOs.Store;
using FC.Api.DTOs.Store.Releasing;
using FC.Api.DTOs.Store.Returns;
using FC.Api.Helpers;
using FC.Api.Helpers.Notification;
using FC.Api.Services.Stores;
using FC.Api.Services.Stores.Releasing;
using FC.Api.Validators.Store;
using FC.Api.Validators.Store.Releasing;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FC.Api.Controllers
{

    [Authorize]
    [Produces("application/json")]
    [Route("api/transactions/releasing")]
    public class ReleasingController : BaseController
    {

        private ISTSalesService _salesService;
        private ISTStockService _stockService;
        private ISTDeliveryService _deliveryService;


        private IForClientOrderService _forClientOrderService;
        private ISameDaySalesService _sameDaySalesService;
        private ISalesOrderService _salesOrderService;
        private ITransferService _transferService;
        private IPurchaseReturnService _purchaseReturnService;
        private readonly AppSettings _appSettings;


        private IMapper _mapper;

        public ReleasingController(IForClientOrderService forClientOrderService, 
                                   ISameDaySalesService sameDaySalesService,
                                   ISalesOrderService salesOrderService,
                                   ITransferService transferService,
                                   IPurchaseReturnService purchaseReturnService,
                                   ISTSalesService salesService, 
                                   ISTStockService stockService, 
                                   ISTDeliveryService deliveryService, 
                                   IMapper mapper,
                                   IOptions<AppSettings> appSettings)
        {
            _forClientOrderService = forClientOrderService;
            _sameDaySalesService = sameDaySalesService;
            _salesOrderService = salesOrderService;
            _transferService = transferService;
            _purchaseReturnService = purchaseReturnService;

            _salesService = salesService;
            _stockService = stockService;
            _deliveryService = deliveryService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }


        #region For Client Order

        [Route("forclientorder")]
        public IActionResult GetAllReleasing_ForClientOrder(SearchForClientOrder search)
        {
            search.StoreId = this.GetCurrentUserStoreId(_forClientOrderService.DataContext());

            //var list = _forClientOrderService.GetAll(search);

            var list = _forClientOrderService.GetAllPaged(search, _appSettings);

            return Ok(list);
        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [Route("forclientorder/{id}")]
        public IActionResult Release_ForClientOrder(int id, [FromBody]ReleaseForClientOrderDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            dto.StoreId = this.GetCurrentUserStoreId(_forClientOrderService.DataContext());
            if (!dto.StoreId.HasValue)
            {
                return Unauthorized();
            }

            var validator = new ReleaseForClientOrderDTOValidator(_forClientOrderService.DataContext());

            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                return BadRequest(GetErrorMessages(results));
            }

            var obj = _mapper.Map<STSales>(dto);
            _forClientOrderService.Update(_stockService, obj);

            return Ok(obj);
        }


        #endregion

        #region Same Day Sales

        [Route("samedaysales")]
        public IActionResult GetAllReleasing_SameDaySales(SearchSameDaySales search)
        {
            search.StoreId = this.GetCurrentUserStoreId(_sameDaySalesService.DataContext());

            //var list = _sameDaySalesService.GetAll(search);
            var list = _sameDaySalesService.GetAllPaged(search, _appSettings);

            return Ok(list);
        }
        [NotifyActionFilter(groupNum: AssignmentEnum.Store)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [Route("samedaysales/{id}")]
        public IActionResult Update_SameDaySales(int id, [FromBody]UpdateSameDaySalesDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            dto.StoreId = this.GetCurrentUserStoreId(_sameDaySalesService.DataContext());
            if (!dto.StoreId.HasValue)
            {
                return Unauthorized();
            }

            var validator = new UpdateSameDaySalesDTOValidator(_sameDaySalesService.DataContext());

            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                return BadRequest(GetErrorMessages(results));
            }

            var obj = _mapper.Map<STSales>(dto);
            _sameDaySalesService.Update(_stockService, obj);

            return Ok(obj);
        }

        #endregion

        #region Sales Order

        [Route("salesorder")]
        public IActionResult GetAllReleasing_SalesOrder(DTOs.Store.Releasing.SearchSalesOrder search)
        {
            search.StoreId = this.GetCurrentUserStoreId(_salesOrderService.DataContext());

            var list = _salesOrderService.GetAllPaged(search, _appSettings);
            //var list = _salesOrderService.GetAll(search);

            return Ok(list);
        }

        [NotifyActionFilter(groupNum: AssignmentEnum.Logistics)]
        [NotifyActionFilter(groupNum: AssignmentEnum.Store)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [Route("salesorder/{id}")]
        public IActionResult Release_SalesOrder_Pickup(int id)
        {
            var storeId = this.GetCurrentUserStoreId(_salesService.DataContext());
            if (!storeId.HasValue)
            {
                return Unauthorized();
            }

            var obj = _salesOrderService.GetSalesRecordByIdAndStoreId(id, storeId);
            if (obj == null)
            {
                return BadRequest();
            }

            _salesOrderService.UpdateSalesRecord(_stockService, obj);

            return Ok(obj);
        }

        [NotifyActionFilter(groupNum: AssignmentEnum.Logistics)]
        [NotifyActionFilter(groupNum: AssignmentEnum.Store)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [Route("salesorder/delivery/{id}")]
        public IActionResult Release_SalesOrder_Delivery(int id)
        {
            var storeId = this.GetCurrentUserStoreId(_salesService.DataContext());
            if (!storeId.HasValue)
            {
                return Unauthorized();
            }

            var obj = _salesOrderService.GetDeliveryRecordByIdAndStoreId(id, storeId);
            if (obj == null)
            {
                return BadRequest();
            }

            var transactionNo = _salesOrderService.UpdateDelivery(_stockService, obj);

            return Ok(transactionNo);
        }

        #endregion

        #region Transfer

        [Route("transfer")]
        public IActionResult GetAllReleasing_Transfer(DTOs.Store.Releasing.SearchTransfer search)
        {
            search.StoreId = this.GetCurrentUserStoreId(_transferService.DataContext());

            //var list = _transferService.GetAll(search);
            var list = _transferService.GetAllPaged(search, _appSettings);

            return Ok(list);
        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [Route("transfer/{id}")]
        public IActionResult Release_Transfer_Pickup(int id)
        {
            var storeId = this.GetCurrentUserStoreId(_transferService.DataContext());
            if (!storeId.HasValue)
            {
                return Unauthorized();
            }

            var obj = _transferService.GetPickupOrderByIdAndStoreId(id, storeId);
            if (obj == null)
            {
                return BadRequest();
            }

            _transferService.UpdatePickupOrder(_stockService, obj);

            return Ok(obj);
        }
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [Route("transfer/delivery/{id}")]
        public IActionResult Release_Transfer_Delivery(int id)
        {
            var storeId = this.GetCurrentUserStoreId(_transferService.DataContext());
            if (!storeId.HasValue)
            {
                return Unauthorized();
            }

            var obj = _transferService.GetDeliveryOrderByIdAndStoreId(id, storeId);
            if (obj == null)
            {
                return BadRequest();
            }

            _transferService.UpdateDeliveryOrder(_stockService, obj);

            return Ok(obj);
        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [Route("transfer/showroom/{id}")]
        public IActionResult Release_Showroom_Delivery(int id)
        {
            var storeId = this.GetCurrentUserStoreId(_transferService.DataContext());
            if (!storeId.HasValue)
            {
                return Unauthorized();
            }

            var obj = _transferService.GetShowroomPickupOrderByIdAndStoreId(id, storeId);
            if (obj == null)
            {
                return BadRequest();
            }

            var transNo = _transferService.UpdateShowroomPickupOrder(_stockService, obj);

            return Ok(transNo);
        }

        #endregion

        #region Returns

        [Route("returns")]
        public IActionResult GetAllReleasing_PurchaseReturn(SearchReturns search)
        {
            search.StoreId = this.GetCurrentUserStoreId(_purchaseReturnService.DataContext());

            //var list = _purchaseReturnService.GetAll(search);
            var list = _purchaseReturnService.GetAllPaged(search, _appSettings);

            return Ok(list);
        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [Route("returns/{id}")]
        public IActionResult Release_PurchaseReturn_Delivery(int id, [FromBody]ReleaseReturnsDTO release)
        {
            var storeId = this.GetCurrentUserStoreId(_transferService.DataContext());
            if (!storeId.HasValue)
            {
                return Unauthorized();
            }

            var obj = _purchaseReturnService.GetDeliveryByIdAndStoreId(id, storeId);
            if (obj == null)
            {
                return BadRequest();
            }

            _purchaseReturnService.ReleasePurchaseReturnDelivery(_stockService, obj);

            return Ok(release);
        }

        #endregion




        [HttpGet]
        public IActionResult GetAllSalesForReleasing(SearchSalesForReleasing search)
        {
            search.StoreId = this.GetCurrentUserStoreId(_salesService.DataContext());

            var obj = _salesService.GetAllSalesForReleasing(search, _mapper);

            return Ok(obj);
        }


        /// <summary>
        /// Used for sales records with deliverytype != delivery
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetSalesForReleasingById(int? id)
        {
            var storeId = this.GetCurrentUserStoreId(_salesService.DataContext());

            var obj = _salesService.GetSalesForReleasingById(id, storeId);
            if (obj == null)
            {
                return BadRequest();
            }

            var dto = _mapper.Map<STSalesDTO>(obj);
            return Ok(dto);
        }

        /// <summary>
        /// Used for sales records with deliverytype != delivery
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// 
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut("{id}")]
        public IActionResult UpdateSalesForReleasing(int id, [FromBody]UpdateSalesForReleasingDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            dto.StoreId = this.GetCurrentUserStoreId(_salesService.DataContext());
            if (!dto.StoreId.HasValue)
            {
                return Unauthorized();
            }

            var validator = new UpdateSalesForReleasingDTOValidator(_salesService.DataContext());

            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                return BadRequest(GetErrorMessages(results));
            }

            var obj = _mapper.Map<STSales>(dto);
            _salesService.UpdateSalesForReleasing(_stockService, obj);

            return Ok(obj);
        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut]
        [Route("delivery/{id}")]
        public IActionResult UpdateSalesDeliveryForReleasing(int id)
        {
            var storeId = this.GetCurrentUserStoreId(_salesService.DataContext());
            if (!storeId.HasValue)
            {
                return Unauthorized();
            }

            var obj = _deliveryService.GetSalesDeliveryForReleasingByIdAndStoreId(id, storeId);
            if (obj == null)
            {
                return BadRequest();
            }

            _deliveryService.UpdateSalesDeliveryForReleasing(_deliveryService, _stockService, obj);

            return Ok(obj);
        }

        #region Release Item

        #region Sales
        [NotifyActionFilter(groupNum: AssignmentEnum.Store)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPost]
        [Route("sales")]
        public IActionResult AddSales([FromBody]AddSalesDTO dto)
        {
            dto.StoreId = this.GetCurrentUserStoreId(_salesService.DataContext());
            if (!dto.StoreId.HasValue)
            {
                return BadRequest();
            }

            foreach (var item in dto.SoldItems)
            {
                item.StoreId = dto.StoreId;
            }

            var validator = new AddSalesDTOValidator(_salesService.DataContext());

            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                return BadRequest(GetErrorMessages(results));
            }

            var obj = _mapper.Map<STSales>(dto);
            obj.SalesType = SalesTypeEnum.Releasing;
            obj.DeliveryType = DeliveryTypeEnum.Pickup;
            _salesService.InsertSales(obj, true,dto.IsSameDaySales);
            return Ok(obj);
        }
        

        #endregion

        #endregion

    }

}