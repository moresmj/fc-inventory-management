using AutoMapper;
using FC.Api.DTOs.Store;
using FC.Api.DTOs.Store.AdvanceOrder;
using FC.Api.DTOs.Store.AdvanceOrders;
using FC.Api.DTOs.Warehouse.AllocateAdvanceOrder;
using FC.Api.Helpers;
using FC.Api.Helpers.Notification;
using FC.Api.Services.Stores;
using FC.Api.Services.Stores.AdvanceOrder;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Users;
using FC.Core.Domain.Warehouses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace FC.Api.Controllers
{
    [Route("api/transactions/orders")]
    public class OrdersController : BaseController
    {

        private ISTOrderService _orderservice;
        private ISTDeliveryService _deliveryService;
        private ISTStockService _stStockService;
        private ISTAdvanceOrderService _stAdvanceOrderService;
        private readonly AppSettings _appSettings;
        private IMapper _mapper;

        public OrdersController(ISTOrderService orderservice,
                                ISTDeliveryService deliveryService,
                                ISTStockService stockService,
                                ISTAdvanceOrderService sTAdvanceOrderService,
                                IMapper mapper,
                                IOptions<AppSettings> appSettings
                                )
        {
            _orderservice = orderservice;
            _deliveryService = deliveryService;
            _stStockService = stockService;
            _stAdvanceOrderService = sTAdvanceOrderService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        
        #region Create New Order

        #region Showroom Stock

        [HttpGet]
        [Route("showroomstock/{id}")]
        public IActionResult GetReceiveItemFromOrdersById(int? id)
        {
            var storeId = this.GetCurrentUserStoreId(_orderservice.DataContext());

            var obj = _orderservice.GetOrderByIdAndStoreId(id, storeId);
            if (obj == null)
            {
                return BadRequest();
            }

            var dto = _mapper.Map<STOrderDTO>(obj);
            return Ok(dto);
        }

        [NotifyActionFilter(groupNum: AssignmentEnum.MainOffice)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPost]
        [Route("showroomstock")]
        public IActionResult AddShowroomStock([FromBody] STOrderDTO dto)
        {
            var storeId = this.GetCurrentUserStoreId(_orderservice.DataContext());
           
            if(!storeId.HasValue)
            {
                return BadRequest();
            }

            var obj = _mapper.Map<STOrder>(dto);

            //  Set order type to determine that it's for Showroom Stock
            obj.OrderType = OrderTypeEnum.ShowroomStockOrder;


            var userType = this.GetCurrentUserUserType(_orderservice.DataContext());

            //added for dealer user will mark order.
            if (userType == UserTypeEnum.Dealer)
            {
                obj.isDealerOrder = true;

            }
            obj.DeliveryType = DeliveryTypeEnum.Delivery;


            //  Set storeid
            obj.StoreId = storeId;
            
            _orderservice.InsertOrder(obj, _appSettings);
            return Ok(obj);
        }

        #endregion

        #region For Client
        [NotifyActionFilter(groupNum: AssignmentEnum.Store)]
        [NotifyActionFilter(groupNum : AssignmentEnum.MainOffice)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPost]
        [Route("forclient")]
        public IActionResult AddForClientOrder([FromBody] ForClientOrderDTO dto)
        {
            var storeId = this.GetCurrentUserStoreId(_orderservice.DataContext());
            if (!storeId.HasValue)
            {
                return BadRequest();
            }

            var obj = _mapper.Map<STOrder>(dto);

            //  Set order type to determine that it's for Client Order
            obj.OrderType = OrderTypeEnum.ClientOrder;

            var userType = this.GetCurrentUserUserType(_orderservice.DataContext());

            //added for dealer user will mark order.
            if (userType == UserTypeEnum.Dealer)
            {
                obj.isDealerOrder = true;

            }

            //  Set storeid
            obj.StoreId = storeId;

            _orderservice.InsertOrder(obj, _appSettings);
            return Ok(obj);
        }

        #endregion

        #region Advance order

        //[ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPost]
        [Route("advanceorder")]
        public IActionResult AddAdvanceOrder([FromBody] STAdvanceOrderDTO dto)
        {

            var storeId = this.GetCurrentUserStoreId(_orderservice.DataContext());
            if (!storeId.HasValue)
            {
                return BadRequest();
            }

            dto.StoreId = storeId;

            var obj = _mapper.Map<STAdvanceOrder>(dto);

            _orderservice.InsertAdvanceOrder(obj);

            return Ok(obj);
        }
        #endregion

        #endregion


        [HttpGet]
        public IActionResult GetAllOrders(OrderSearch search)
        {
            search.StoreId = this.GetCurrentUserStoreId(_orderservice.DataContext());

            //return Ok(_orderservice.GetAllOrders2(search));
            return Ok(_orderservice.GetAllOrders3(search,_appSettings));
        }


        [HttpPut]
        [Route("addclientsi/{id}")]
        public IActionResult AssignClientSI([FromBody]AddClientSIDTO dto)
        {
            var StoreId = this.GetCurrentUserStoreId(_orderservice.DataContext());

            if (!StoreId.HasValue)
            {
                return BadRequest();
            }

            _orderservice.AddClientSINumber(dto, StoreId);

            return Ok();
        }


        #region Delivery

        [HttpGet]
        [Route("delivery/{id}")]
        public IActionResult GetDeliveryById(int? id)
        {
            var storeId = this.GetCurrentUserStoreId(_orderservice.DataContext());

            var obj = _orderservice.GetOrderByIdAndStoreId(id, storeId);

            if (obj == null)
            {
                return BadRequest();
            }

            var dto = _mapper.Map<STOrderDTO>(obj);
            return Ok(dto);
        }

        #region Showroom

        [NotifyActionFilter(groupNum: AssignmentEnum.Logistics)]
        [NotifyActionFilter(groupNum: AssignmentEnum.Store)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut]
        [Route("delivery/showroom/{id}")]
        public IActionResult AddShowroomDelivery(int id, [FromBody] AddShowroomDeliveryDTO dto)
        {
            if (dto.Id != id)
            {
                return BadRequest();
            }

            var storeId = this.GetCurrentUserStoreId(_orderservice.DataContext());

            var obj = _orderservice.GetShowroomDeliveriesByIdAndStoreId(id, storeId);
            if (obj == null)
            {
                return BadRequest();
            }

            var dt = _mapper.Map<STDelivery>(dto);

            dt.StoreId = storeId;

            _deliveryService.InsertDeliveryToShowroom(dt);

            return Ok(obj);
        }

        [HttpGet]
        [Route("delivery/showroom/{id}")]
        public IActionResult GetShowroomDeliveriesById(int? id)
        {
            var storeId = this.GetCurrentUserStoreId(_orderservice.DataContext());

            var obj = _orderservice.GetShowroomDeliveriesByIdAndStoreId(id, storeId);
            if (obj == null)
            {
                return BadRequest();
            }

            //var dto = _mapper.Map<STOrderDTO>(obj);
            return Ok(obj);
        }

        #endregion

        #region Client
        [NotifyActionFilter(groupNum: AssignmentEnum.Logistics)]
        [NotifyActionFilter(groupNum: AssignmentEnum.Store)]
        [NotifyActionFilter(groupNum: AssignmentEnum.Warehouse)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut]
        [Route("delivery/client/{id}")]
        public IActionResult AddClientDelivery(int id, [FromBody] AddClientDeliveryDTO dto)
        {
            if (dto.Id != id)
            {
                return BadRequest();
            }

            var storeId = this.GetCurrentUserStoreId(_orderservice.DataContext());

            var obj = _orderservice.GetClientDeliveriesByIdAndStoreId(id, storeId);
            if (obj == null)
            {
                return BadRequest();
            }

            var dt = _mapper.Map<STDelivery>(dto);

            dt.StoreId = storeId;

            _deliveryService.InsertDeliveryForClient(dt);

            return Ok(obj);
        }

        [HttpGet]
        [Route("delivery/client/{id}")]
        public IActionResult GetClientDeliveriesById(int? id)
        {
            var storeId = this.GetCurrentUserStoreId(_orderservice.DataContext());

            var obj = _orderservice.GetClientDeliveriesByIdAndStoreId(id, storeId);
            if (obj == null)
            {
                return BadRequest();
            }

            //var dto = _mapper.Map<STOrderDTO>(obj);
            return Ok(obj);
        }

        #endregion

        #endregion


        #region Receive Items

        [HttpGet]
        [Route("receiveitems")]
        public IActionResult GetAllReceiveItems(SearchReceiveItems search)
        {
            search.StoreId = this.GetCurrentUserStoreId(_orderservice.DataContext());

            //var list = _orderservice.GetAllForReceiving(search);

            //var obj = _mapper.Map<IList<STOrderDTO>>(list);

            var obj = _orderservice.GetAllForReceiving2(search);
            return Ok(obj);
        }

        [HttpGet("{id}")]
        [Route("receiveitems/{id}")]
        public IActionResult GetReceiveItemByItem(int? id)
        {
            var storeId = this.GetCurrentUserStoreId(_orderservice.DataContext());

            var obj = _orderservice.GetReceivingItemByIdAndStoreId(id, storeId);
            if (obj == null)
            {
                return BadRequest();
            }

            return Ok(obj);
        }
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut]
        [Route("receiveitems/{id}")]
        public IActionResult SaveReceiveItem(int id, [FromBody] SaveReceiveItem dto)
        {
            if (dto.Id != id)
            {
                return BadRequest();
            }

            var storeId = this.GetCurrentUserStoreId(_orderservice.DataContext());

            var obj = _orderservice.GetReceivingItemByIdAndStoreId(id, storeId);
            if (obj == null)
            {
                return BadRequest();
            }

            _deliveryService.InsertToSTStock(dto, _stStockService,_orderservice);
            return Ok(obj);
        }


        #endregion

        [HttpGet]
        [Route("warehouse/advanceorder")]
        public IActionResult GetApprovedAdvanceOrders(SearchApproveRequests search)
        {
            var WarehouseId = this.GetCurrentUserWarehouseId(_orderservice.DataContext());

            var StoreId = this.GetCurrentUserStoreId(_orderservice.DataContext());

            if(!WarehouseId.HasValue && !StoreId.HasValue)
            {
                return BadRequest();
            }
            search.WarehouseId = WarehouseId;
            search.StoreId = StoreId;

            var advanceOrders = _stAdvanceOrderService.GetApprovedAdvanceOrderList(search, _appSettings);

            return Ok(advanceOrders);

            
        }

        [HttpGet]
        [Route("warehouse/advanceorder/details/{id}")]
        public IActionResult GetAdvanceOrderById(int? id)
        {
            var warehouseId = this.GetCurrentUserWarehouseId(_orderservice.DataContext());

            var obj = _stAdvanceOrderService.GetAdvanceOrderById(id);

            if (obj == null)
            {
                return BadRequest();
            }


            return Ok(obj);
        }

        [HttpPut]
        [Route("warehouse/advanceorder/allocate/{id}")]
        public IActionResult AllocateAdvanceOrder(int? id,[FromBody] AllocateAdvanceOrderDTO order)
        {
            if(id == null)
            {
               return BadRequest();
            }

            var obj = _mapper.Map<WHAllocateAdvanceOrder>(order);

            obj.WarehouseId = this.GetCurrentUserWarehouseId(_orderservice.DataContext());

            _stAdvanceOrderService.AllocateAdvanceOrder(id, obj, _appSettings);

            return Ok();
        }

        [HttpPut]
        [Route("warehouse/advanceorder/modifydelivery/{id}")]
        public IActionResult ModifyAdvanceOrderDelivery([FromBody] ModifyAdvanceOrderDTO order)
        {
            var userType = this.GetCurrentUserUserType(_orderservice.DataContext());
            var isDealer =false;
            if (userType == UserTypeEnum.Dealer)
            {
                isDealer = true;
            }

            var poNumber = _stAdvanceOrderService.UpdateDelivery(order, _appSettings, isDealer);

            return Ok(poNumber);
        }



    }
}