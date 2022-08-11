using AutoMapper;
using FC.Api.DTOs.Store.ReceiveItems;
using FC.Api.DTOs.Warehouse;
using FC.Api.DTOs.Warehouse.Receive_Items;
using FC.Api.Helpers;
using FC.Api.Helpers.Notification;
using FC.Api.Services.Stores.ReceiveItems;
using FC.Api.Services.Warehouses;
using FC.Api.Services.Warehouses.Receive_Items;
using FC.Api.Validators.Store.ReceiveItems;
using FC.Api.Validators.Warehouse.Receive_Items;
using FC.Core.Domain.Users;
using FC.Core.Domain.Warehouses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace FC.Api.Controllers
{
    [Route("api/transactions/receiveitems")]
    public class ReceiveItemsController : BaseController
    {

        private IWHReceiveService _whReceiveservice;
        private IWHStockService _whStockService;
        private IReturnsForReceivingService _returnsForReceivingService;
        private IClientReturnService _clientReturnService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public ReceiveItemsController(IWHReceiveService whReceiveservice, IWHStockService wHStockService,
                                      IReturnsForReceivingService returnsForReceivingService, IMapper mapper,
                                      IClientReturnService clientReturnService,
                                      IOptions<AppSettings> appSettings)
        {
            _whReceiveservice = whReceiveservice;
            _whStockService = wHStockService;
            _returnsForReceivingService = returnsForReceivingService;
            _mapper = mapper;
            _clientReturnService = clientReturnService;
            _appSettings = appSettings.Value;
        }

        #region From Orders

        [HttpGet]
        public IActionResult GetAllReceiveItems(WHReceiveSearchDTO dto)
        {
            dto.WarehouseId = this.GetCurrentUserWarehouseId(_whReceiveservice.DataContext());

            //var list = _whReceiveservice.GetAllReceives(dto);
            var list = _whReceiveservice.GetAllReceives2(dto, _appSettings);

            //var obj = _mapper.Map<IList<WHReceiveDTO>>(list);
            return Ok(list);
        }

        [HttpGet]
        [Route("fromorders/{id}")]
        public IActionResult GetReceiveItemById(int? id)
        {
            var obj = _whReceiveservice.GetReceiveByIdAndWarehouseId(id, this.GetCurrentUserWarehouseId(_whReceiveservice.DataContext()));

            if (obj == null)
            {
                return BadRequest();
            }

            var dto = _mapper.Map<WHReceiveDTO>(obj);
            return Ok(dto);
        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPost]
        [Route("fromorders")]
        public IActionResult AddReceiveItem([FromBody] WHReceiveDTO dto)
        {
            var warehouseId = this.GetCurrentUserWarehouseId(_whReceiveservice.DataContext());

            if(!warehouseId.HasValue)
            {
                return BadRequest();
            }

            var obj = _mapper.Map<WHReceive>(dto);

            obj.WarehouseId = warehouseId;

            _whReceiveservice.InsertReceive(obj, _mapper, _whStockService);

            return Ok(obj);
        }

        #endregion

        #region From Returns

        [HttpGet]
        [Route("returns")]
        public IActionResult GetAllReturnsForReceiving(SearchReturnsForReceiving search)
        {
            search.WarehouseId = this.GetCurrentUserWarehouseId(_returnsForReceivingService.DataContext());

            var list = _returnsForReceivingService.GetAll(search);

            return Ok(list);
        }

        [HttpGet]
        [Route("returns/{id}")]
        public IActionResult GetReturnsForReceivingById(int id)
        {
            var warehouseId = this.GetCurrentUserWarehouseId(_returnsForReceivingService.DataContext());

            var record = _returnsForReceivingService.GetReturnsForReceivingByIdAndWarehouseId(id, warehouseId);

            return Ok(record);
        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut]
        [Route("returns/{id}")]
        public IActionResult ReceiveReturns(int id, [FromBody]ReceiveReturnsDTO dto)
        {
            if(id != dto.Id)
            {
                return BadRequest();
            }

            dto.WarehouseId = this.GetCurrentUserWarehouseId(_returnsForReceivingService.DataContext());

            var validator = new ReceiveReturnsDTOValidator(_returnsForReceivingService.DataContext());

            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                return BadRequest(GetErrorMessages(results));
            }

            _returnsForReceivingService.ReceiveReturns(dto);

            return Ok(dto);
        }

        #endregion

        #region ClientReturns

        [HttpGet]
        [Route("clientreturn")]
        public IActionResult GetAll_ReceiveItems_ClientReturns(SearchReceiveItemsClientReturns search)
        {
            search.StoreId = this.GetCurrentUserStoreId(_clientReturnService.DataContext());

            var list = _clientReturnService.GetAll(search);

            return Ok(list);
        }

        [HttpGet]
        [Route("clientreturn/{id}")]
        public IActionResult GetClientReturnsById(int id)
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
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut]
        [Route("clientreturn/{id}")]
        public IActionResult ReceiveClientReturns(int id, [FromBody]ReceiveClientReturnsDTO dto)
        {
            if(id != dto.Id)
            {
                return BadRequest();
            }

            dto.StoreId = this.GetCurrentUserStoreId(_clientReturnService.DataContext());
            foreach(var item in dto.ClientPurchasedItems)
            {
                item.StoreId = dto.StoreId;
            }

            var validator = new ReceiveClientReturnsDTOValidator(_clientReturnService.DataContext());

            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                return BadRequest(GetErrorMessages(results));
            }

            _clientReturnService.ReceiveClientReturns(dto);

            return Ok(dto);
        }

        #endregion

    }
}