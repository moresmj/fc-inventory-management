using AutoMapper;
using FC.Api.DTOs.Store.Returns;
using FC.Api.DTOs.Store.Returns.ClientReturn;
using FC.Api.Helpers;
using FC.Api.Helpers.Notification;
using FC.Api.Services.Stores;
using FC.Api.Services.Stores.Returns;
using FC.Api.Services.Warehouses;
using FC.Api.Validators.Store.Returns;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Users;
using FC.Core.Domain.Warehouses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FC.Api.Controllers
{
    [Route("api/returns")]
    public class ReturnsController : BaseController
    {

        private IStoreService _storeService;
        private IPurchaseReturnService _purchaseReturnService;
        private IReturnService _returnService;
        private IWHDeliveryService _deliveryService;
        private IClientReturnService _clientReturnService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public ReturnsController(IStoreService storeService, IPurchaseReturnService purchaseReturnService,
                                 IWHDeliveryService deliveryService, IClientReturnService clientReturnService,
                                 IReturnService returnService, IMapper mapper,
                                 IOptions<AppSettings> appSettings)
        {
            this._storeService = storeService;
            this._returnService = returnService;
            this._purchaseReturnService = purchaseReturnService;
            this._deliveryService = deliveryService;
            this._clientReturnService = clientReturnService;
            this._mapper = mapper;
            _appSettings = appSettings.Value;
        }


        #region CREATE RETURN REQUEST

        #region Client Return

        [HttpGet]
        [NotifyActionFilter(groupNum: AssignmentEnum.Logistics)]
        [NotifyActionFilter(groupNum: AssignmentEnum.Store)]
        [Route("clientreturn")]
        public IActionResult AddClientReturn(SearchClientReturn search)
        {
            search.StoreId = this.GetCurrentUserStoreId(_clientReturnService.DataContext());

            //var list = _clientReturnService.GetAll(search);
            var list = _clientReturnService.GetAllPaged(search, _appSettings);


            return Ok(list);
        }

        [HttpGet]
        [Route("clientreturn/{id}")]
        public IActionResult GetClientReturnById(int id)
        {
            var storeId = this.GetCurrentUserStoreId(_clientReturnService.DataContext());

            var obj = _clientReturnService.GetByIdAndStoreId(id, storeId);
            if(obj == null)
            {
                return BadRequest();
            }

            return Ok(obj);
        }

        [NotifyActionFilter(groupNum: AssignmentEnum.Store)]
        [NotifyActionFilter(groupNum: AssignmentEnum.Logistics)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut]
        [Route("clientreturn/{id}")]
        public IActionResult AddClientReturn(int id,[FromBody]AddClientReturnDTO dto)
        {
            dto.StoreId = this.GetCurrentUserStoreId(_clientReturnService.DataContext());

            var validator = new AddClientReturnDTOValidator(_clientReturnService.DataContext());

            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                return BadRequest(GetErrorMessages(results));
            }

           var transNo =  _clientReturnService.AddClientReturn(dto);

            return Ok(transNo);
        }

        #endregion


        #region RTV/Purchase Returns

        [HttpGet]
        [Route("purchasereturn/warehousename")]
        public IActionResult GetStoreWarehouseName()
        {
            var storeId = this.GetCurrentUserStoreId(_purchaseReturnService.DataContext());

            var obj = _storeService.GetStoreById(storeId);
            if (obj == null)
            {
                return BadRequest();
            }
            
            return Ok(obj.Warehouse.Name);
        }

        [NotifyActionFilter(groupNum: AssignmentEnum.MainOffice)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPost]
        [Route("purchasereturn")]
        public IActionResult AddPurchaseReturn([FromBody]AddPurchaseReturnDTO dto)
        {
            dto.StoreId = this.GetCurrentUserStoreId(_purchaseReturnService.DataContext());

            foreach(var retItem in dto.PurchasedItems)
            {
                retItem.StoreId = dto.StoreId;
            }

            var validator = new AddPurchaseReturnDTOValidator(_purchaseReturnService.DataContext());

            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                return BadRequest(GetErrorMessages(results));
            }

            var obj = _mapper.Map<STReturn>(dto);
            obj.ReturnType = ReturnTypeEnum.RTV;
            _purchaseReturnService.AddPurchaseReturn(obj);

            return Ok(obj);
        }

        #endregion



        #region Breakage

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPost]
        [Route("breakage")]
        public IActionResult AddBreakage([FromBody]AddBreakageDTO dto)
        {
            dto.StoreId = this.GetCurrentUserStoreId(_purchaseReturnService.DataContext());

            foreach (var retItem in dto.PurchasedItems)
            {
                retItem.StoreId = dto.StoreId;
            }

            var validator = new AddBreakageDTOValidator(_purchaseReturnService.DataContext());

            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                return BadRequest(GetErrorMessages(results));
            }

            var obj = _mapper.Map<STReturn>(dto);
            obj.ReturnType = ReturnTypeEnum.Breakage;
            _purchaseReturnService.AddBreakage(obj);

            return Ok(obj);
        }

        #endregion

        #endregion


        public IActionResult GetAllReturns(SearchReturns search)
        {
            search.StoreId = this.GetCurrentUserStoreId(_purchaseReturnService.DataContext());

            //var list = _returnService.GetAllReturns(search);
            var list = _returnService.GetAllReturnsPaged(search, _appSettings);

            return Ok(list);
        }

        #region DELIVERY

        #region RTV/Purchase Return

        [HttpGet]
        [Route("rtv/delivery/{id}")]
        public IActionResult GetPurchaseReturnDeliveryRecordsById(int? id)
        {
            var storeId = this.GetCurrentUserStoreId(_purchaseReturnService.DataContext());

            var obj = _purchaseReturnService.GetPurchaseReturnDeliveryRecordsByIdAndStoreId(id, storeId);
            if (obj == null)
            {
                return BadRequest();
            }

            return Ok(obj);
        }

        [NotifyActionFilter(groupNum: AssignmentEnum.Logistics)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut]
        [Route("rtv/delivery/{id}")]
        public IActionResult AddPurchaseReturnDelivery(int id, [FromBody]AddPurchaseReturnDeliveryDTO dto)
        {
            if (dto.Id != id)
            {
                return BadRequest();
            }

            var storeId = this.GetCurrentUserStoreId(_purchaseReturnService.DataContext());

            var obj = _purchaseReturnService.GetPurchaseReturnByIdAndStoreId(id, storeId);
            if (obj == null)
            {
                return BadRequest();
            }

            var dt = _mapper.Map<WHDelivery>(dto);

            dt.StoreId = storeId;

            _deliveryService.InsertPurchaseReturnDelivery(dt);

            return Ok(obj);
        }

        [HttpGet]
        [Route("rtv/main")]
        public IActionResult GetMainSelectedStoreRTV(SearchReturns search)
        {
            //var obj = _returnService.GetMainSelectedStoreRTV(search, _appSettings);
            var obj = _returnService.GetMainSelectedStoreRTV2(search, _appSettings);

            return Ok(obj);
        }


        #endregion

        #endregion

    }
}