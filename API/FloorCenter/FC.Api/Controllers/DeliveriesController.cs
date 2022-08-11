using AutoMapper;
using FC.Api.DTOs.Store;
using FC.Api.DTOs.Store.Deliveries;
using FC.Api.DTOs.Warehouse.Delivery;
using FC.Api.Helpers;
using FC.Api.Helpers.Notification;
using FC.Api.Services.Stores;
using FC.Api.Services.Stores.Deliveries;
using FC.Api.Services.Warehouses;
using FC.Api.Services.Warehouses.Returns;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Users;
using FC.Core.Domain.Warehouses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FC.Api.Controllers
{
    [Route("api/Deliveries")]
    public class DeliveriesController : BaseController
    {


        private ISTOrderService _orderService;
        private ISTDeliveryService _deliveryService;
        private IReturnsForDeliveriesService _returnsForDeliveriesService;
        private IMapper _mapper;
        private IStoreReturnsForDeliveryService _storeReturnsForDeliveryService;
        private ISTStockService _stockService;
        private readonly AppSettings _appSettings;

        public DeliveriesController(ISTOrderService orderService, ISTDeliveryService deliveryService,
                                    IReturnsForDeliveriesService returnsForDeliveriesService, IMapper mapper,
                                    IStoreReturnsForDeliveryService storeReturnsForDeliveryService, ISTStockService stockService,
                                    IOptions<AppSettings> appSettings)
        {
            _orderService = orderService;
            _deliveryService = deliveryService;
            _returnsForDeliveriesService = returnsForDeliveriesService;
            _storeReturnsForDeliveryService = storeReturnsForDeliveryService;
            _stockService = stockService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }


        [HttpGet]
        public IActionResult GetAllForDeliveries(SearchDeliveries search)
        {
            //var list = _deliveryService.GetAllForDeliveries(search, _mapper);
            //var list = _deliveryService.GetAllForDeliveriesPaged(search, _mapper, _appSettings);
            var list = _deliveryService.GetAllForDeliveriesPaged2(search, _mapper, _appSettings);
            return Ok(list);
        }

        [HttpGet("DeliveryShowroom")]
        public IActionResult GetDeliveriesShowroom(SearchDeliveries search)
        {
            //var list = _deliveryService.GetAllDeliveriesForShowroom(search, _mapper, _appSettings);
            var list = _deliveryService.GetAllForDeliveriesPaged2(search, _mapper, _appSettings);
            return Ok(list);

        }

        [HttpGet("sales")]
        public IActionResult GetAllDeliveriesForSales(SearchDeliveries search)
        {
            //var list = _deliveryService.GetAllDeliveriesForSales(search, _mapper);
            var list = _deliveryService.GetAllDeliveriesForSalesPaged(search, _mapper, _appSettings);
            return Ok(list);
        }


        [HttpGet("{id}")]
        public IActionResult GetDeliveryByItem(int? id)
        {
            var obj = _orderService.GetDeliveryById(id);
            if (obj == null)
            {
                return BadRequest();
            }
            
            var dto = _mapper.Map<STOrderDTO>(obj);
            return Ok(dto);
        }

        [NotifyActionFilter(groupNum: AssignmentEnum.Logistics)]
        [NotifyActionFilter(groupNum: AssignmentEnum.Store)]
        [NotifyActionFilter(groupNum : AssignmentEnum.Warehouse)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut]
        [Route("{id}")]
        public IActionResult UpdateForDelivery(int? id, [FromBody] UpdateForDeliveryDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var obj = _mapper.Map<STDelivery>(dto);

            _deliveryService.UpdateDelivery(obj);
            return Ok(dto);
        }

        [NotifyActionFilter(groupNum: AssignmentEnum.Store)]
        [NotifyActionFilter(groupNum: AssignmentEnum.Warehouse)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut("delivery_update/{id}")]
        public IActionResult UpdateDeliveryStatus(int? id, [FromBody] UpdateDeliveryStatusDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var obj = _mapper.Map<STDelivery>(dto);

            _deliveryService.UpdateDeliveryStatus(_stockService, obj);
            return Ok(dto);
        }


        [HttpGet]
        [Route("returns")]
        public IActionResult GetAllReturnsForDeliveries(SearchReturnsForDeliveries search)
        {
            //var list = _returnsForDeliveriesService.GetAll(search);
            //var list = _returnsForDeliveriesService.GetAllPaged(search, _appSettings);
            var list = _returnsForDeliveriesService.GetAllPaged2(search, _appSettings);
            return Ok(list);
        }

        [NotifyActionFilter(groupNum: AssignmentEnum.Logistics)]
        [NotifyActionFilter(groupNum: AssignmentEnum.Store)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut]
        [Route("returns/{id}")]
        public IActionResult UpdateReturnsDelivery(int? id, [FromBody]UpdateReturnsDeliveryDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }
            
            _returnsForDeliveriesService.UpdateReturnsDelivery(dto);
            return Ok(dto);
        }


        [HttpGet]
        [Route("storereturns")]
        public IActionResult GetAllStoreReturnsForDeliveries(SearchReturnsForDeliveries search)
        {
            //var list = _storeReturnsForDeliveryService.GetAll(search);
            var list = _storeReturnsForDeliveryService.GetAllPaged(search, _appSettings);
            return Ok(list);
        }

        [NotifyActionFilter(groupNum: AssignmentEnum.Logistics)]
        [NotifyActionFilter(groupNum: AssignmentEnum.Store)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut]
        [Route("storereturns/{id}")]
        public IActionResult UpdateStoreReturnsDelivery(int? id, [FromBody]UpdateStoreReturnsDeliveryDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            _storeReturnsForDeliveryService.UpdateStoreReturnsDelivery(dto);
            return Ok(dto);
        }

    }
}