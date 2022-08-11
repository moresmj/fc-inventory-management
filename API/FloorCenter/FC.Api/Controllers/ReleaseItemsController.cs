using System.Collections.Generic;
using AutoMapper;
using FC.Api.DTOs.Store;
using FC.Api.DTOs.Warehouse.ModifyTonality;
using FC.Api.Helpers;
using FC.Api.Helpers.Notification;
using FC.Api.Services.Stores;
using FC.Api.Services.Warehouses;
using FC.Core.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FC.Api.Controllers
{

    /// <summary>
    /// This is for Warehouse
    /// </summary>
    [Route("api/transactions/releaseitems")]
    public class ReleaseItemsController : BaseController
    {

        private ISTOrderService _orderService;
        private ISTSalesService _salesService;
        private ISTStockService _stockService;
        private IWHReceiveService _whReceiveservice;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public ReleaseItemsController(ISTOrderService orderService,
            ISTSalesService salesService,
            ISTStockService stockService,
            IWHReceiveService whReceiveservice,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _orderService = orderService;
            _salesService = salesService;
            _stockService = stockService;
            _whReceiveservice = whReceiveservice;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }


        [HttpGet]
        public IActionResult GetAllForReleasing(SearchReleasing search)
        {
            search.WarehouseId = this.GetCurrentUserWarehouseId(_orderService.DataContext());

            //var list = _orderService.GetAllForReleasing(search);
            var list = _orderService.GetAllForReleasingPaged(search, _appSettings);

            //var obj = _mapper.Map<IList<STOrderDTO>>(list);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public IActionResult GetReleasingById(int? id)
        {
            var warehouseId = this.GetCurrentUserWarehouseId(_orderService.DataContext());

            var obj = _orderService.GetReleasingById(id, warehouseId);
            if (obj == null)
            {
                return BadRequest();
            }

            var dto = _mapper.Map<STOrderDTO>(obj);
            return Ok(dto);
        }

        [NotifyActionFilter(groupNum: AssignmentEnum.Store)]
        [NotifyActionFilter(groupNum: AssignmentEnum.Warehouse)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut("{id}")]
        public IActionResult UpdateForReleasing(int id)
        {
            var warehouseId = this.GetCurrentUserWarehouseId(_orderService.DataContext());

            var obj = _orderService.GetReleasingById(id, warehouseId);

            if (obj == null)
            {
                return BadRequest();
            }

            _orderService.UpdateForReleasing(id, warehouseId);

            return Ok(obj);
        }


        #region OrderType = ClientOrder

        [HttpGet()]
        [Route("clientorder/{id}")]
        public IActionResult GetClientOrderReleasingById(int? id)
        {
            var warehouseId = this.GetCurrentUserWarehouseId(_orderService.DataContext());

            var obj = _orderService.GetClientOrderReleasingByIdAndWarehouseId(id, warehouseId);
            if (obj == null)
            {
                return BadRequest();
            }

            var dto = _mapper.Map<STOrderDTO>(obj);
            return Ok(dto);
        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut()]
        [Route("clientorder/{id}")]
        public IActionResult UpdateClientOrderForReleasing(int id)
        {
            var warehouseId = this.GetCurrentUserWarehouseId(_orderService.DataContext());

            var obj = _orderService.GetClientOrderReleasingByIdAndWarehouseId(id, warehouseId);
            if (obj == null)
            {
                return BadRequest();
            }

            _orderService.UpdateClientOrderForReleasing(obj, _salesService, _stockService, id, warehouseId);

            return Ok(obj);
        }

        #endregion

        [HttpGet]
        [Route("changetonality")]
        public IActionResult GetAllForTonalityChanging(SearchReleasing search)
        {
            search.WarehouseId = this.GetCurrentUserWarehouseId(_orderService.DataContext());


            var list = _orderService.GetAllForTonalityReplacement(search, _appSettings);

            return Ok(list);
        }


        [HttpPost()]
        [Route("changetonality")]
        public IActionResult ModifyItemTonality([FromBody] WHModifyItemTonalityDTO record)
        {
            var warehouseId = this.GetCurrentUserWarehouseId(_whReceiveservice.DataContext());
            record.WarehouseId = warehouseId;

            _orderService.AddModifyItemTonality(record);
            return Ok();

        }


    }
}