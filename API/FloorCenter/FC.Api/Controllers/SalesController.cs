using AutoMapper;
using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Api.Helpers.Notification;
using FC.Api.Services.Stores;
using FC.Api.Validators.Store;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FC.Api.Controllers
{

    [Route("api/transactions/salesorder")]
    public class SalesController : BaseController
    {
        private ISTStockService _stockService;
        private ISTSalesService _salesService;
        private ISTDeliveryService _deliveryService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;


        public SalesController(ISTStockService stockService,
                                ISTSalesService salesService,
                                ISTDeliveryService deliveryService,
                                IMapper mapper,
                                IOptions<AppSettings> appSettings)
        {
            _stockService = stockService;
            _salesService = salesService;
            _deliveryService = deliveryService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        #region Add New Sales Order
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPost]
        public IActionResult AddSalesOrder([FromBody]AddSalesOrderDTO dto)
        {
            dto.StoreId = this.GetCurrentUserStoreId(_salesService.DataContext());
            if(!dto.StoreId.HasValue)
            {
                return Unauthorized();
            }

            foreach (var item in dto.SoldItems)
            {
                item.StoreId = dto.StoreId;
            }

            var validator = new AddSalesOrderDTOValidator(_salesService.DataContext());

            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                return BadRequest(GetErrorMessages(results));
            }

            var obj = _mapper.Map<STSales>(dto);
            obj.SalesType = SalesTypeEnum.SalesOrder;
            if(obj.DeliveryType == DeliveryTypeEnum.Delivery)
            {
                _salesService.InsertSales(obj);
            }
            else
            {
                _salesService.InsertSales(obj, true);
            }

            return Ok(obj);
        }

        #endregion

        [HttpGet]
        public IActionResult GetSalesOrder(SearchSalesOrder search)
        {
            search.StoreId = this.GetCurrentUserStoreId(_salesService.DataContext());
            if(!search.StoreId.HasValue)
            {
                return Unauthorized();
            }

            //var list = _salesService.GetAllSalesOrders(search);
            var list = _salesService.GetAllSalesOrdersPaged(search, _appSettings);

            return Ok(list);
        }

        #region Delivery

        [HttpGet()]
        [Route("delivery/{id}")]
        public IActionResult GetSalesOrderDeliveriesBySalesId(int? id)
        {
            var storeId = this.GetCurrentUserStoreId(_salesService.DataContext());

            var obj = _deliveryService.GetSalesOrderDeliveriesBySalesId(id, storeId);
            if (obj == null)
            {
                return BadRequest();
            }

            return Ok(obj);
        }

        [NotifyActionFilter(groupNum: AssignmentEnum.Logistics)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut()]
        [Route("delivery/{id}")]
        public IActionResult AddSalesOrderDelivery(int id, [FromBody] AddSalesOrderDeliveryDTO dto)
        {
            if (dto.Id != id)
            {
                return BadRequest();
            }

            var storeId = this.GetCurrentUserStoreId(_salesService.DataContext());

            var obj = _deliveryService.GetSalesOrderDeliveriesBySalesId(id, storeId);
            if (obj == null)
            {
                return BadRequest();
            }

            var dt = _mapper.Map<STDelivery>(dto);

            dt.StoreId = storeId;

            _deliveryService.InsertSalesOrderDelivery(_stockService, dt);

            return Ok(obj);

        }

        #endregion

    }

}