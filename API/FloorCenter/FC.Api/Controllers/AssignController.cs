using AutoMapper;
using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Api.Helpers.Notification;
using FC.Api.Services.Stores;
using FC.Api.Services.Warehouses;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Controllers
{
    [Route("api/assign")]
    public class AssignController : BaseController
    {
        private IWHStockService _warehouseStockService;
        private ISTOrderService _service;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public AssignController(IWHStockService whStockService,
            ISTOrderService service,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _warehouseStockService = whStockService;
            _service = service;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [HttpGet("main/whdrnumber")]
        public IActionResult GetOrdersToBeAssignedWithDRNumber(OrderSearch search)
        {
            //var list = _service.GetOrdersToBeAssignedWithDRNumber(search);
            var list = _service.GetOrdersToBeAssignedWithDRNumberPaged(search, _appSettings);
            return Ok(list);
        }
        [NotifyActionFilter(groupNum: AssignmentEnum.Store)]
        [NotifyActionFilter(groupNum: AssignmentEnum.MainOffice)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut("main/whdrnumber/{id}")]
        public IActionResult UpdateRequest(int id, [FromBody]UpdateWHDRNumberDTO dto)
        {
            var userType = this.GetCurrentUserUserType(_warehouseStockService.DataContext());
            if (userType == UserTypeEnum.User || userType == UserTypeEnum.Checker)
            {
                return BadRequest();
            }

            if (id != dto.Id)
            {
                return BadRequest();
            }

            var obj = _mapper.Map<STOrder>(dto);
            _service.UpdateWHDRNumberOfSTOrder(obj, _warehouseStockService);

            return Ok(obj);
        }

    }
}
