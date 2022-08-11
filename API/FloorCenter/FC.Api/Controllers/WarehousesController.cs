using AutoMapper;
using FC.Api.DTOs.Warehouse;
using FC.Api.Helpers;
using FC.Api.Services.Warehouses;
using FC.Core.Domain.Users;
using FC.Core.Domain.Warehouses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FC.Api.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Warehouses")]
    public class WarehousesController : BaseController
    {
        private IWHStockService _warehouseStockService;
        private IWarehouseService _service;
        private IMapper _mapper;

        public WarehousesController(IWHStockService whStockService, IWarehouseService service, IMapper mapper)
        {
            _warehouseStockService = whStockService;
            _service = service;
            _mapper = mapper;
        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPost]
        public IActionResult Add([FromBody]WarehouseDTO dto)
        {
            var userType = this.GetCurrentUserUserType(_warehouseStockService.DataContext());
            if (userType != UserTypeEnum.Administrator)
            {
                return BadRequest();
            }

            var obj = _mapper.Map<Warehouse>(dto);
            _service.InsertWarehouse(obj);
            return Ok(obj);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _service.GetAllWarehouses();

            var obj = _mapper.Map<IList<WarehouseDTO>>(list);
            return Ok(obj);
        }
        [Route("storewarehouse")]
        [HttpGet]
        public IActionResult GetWarehouseList()
        {
            var storeId = this.GetCurrentUserStoreId(_warehouseStockService.DataContext());
            var list = _service.GetWarehouseWithParam(storeId);
            var obj = _mapper.Map<IList<WarehouseDTO>>(list);

            return Ok(obj);
            
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int? id)
        {
            var obj = _service.GetWarehouseById(id);

            if(obj == null)
            {
                return BadRequest();
            }

            var dto = _mapper.Map<WarehouseDTO>(obj);
            return Ok(dto);
        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]WarehouseDTO dto)
        {
            var userType = this.GetCurrentUserUserType(_warehouseStockService.DataContext());
            if (userType != UserTypeEnum.Administrator)
            {
                return BadRequest();
            }

            if (id != dto.Id)
            {
                return BadRequest();
            }

            var obj = _mapper.Map<Warehouse>(dto);
            if (obj == null)
            {
                return BadRequest();
            }

            _service.UpdateWarehouse(obj);
            return Ok(obj);
        }
    }
}