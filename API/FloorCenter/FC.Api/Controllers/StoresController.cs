using System.Collections.Generic;
using AutoMapper;
using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Api.Services.Stores;
using FC.Api.Services.Warehouses;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FC.Api.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Stores")]
    public class StoresController : BaseController
    {
        private IWHStockService _warehouseStockService;
        private IStoreService _service;
        private IMapper _mapper;

        public StoresController(IWHStockService whStockService, IStoreService service, IMapper mapper)
        {
            _warehouseStockService = whStockService;
            _service = service;
            _mapper = mapper;
        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPost]
        public IActionResult Add([FromBody]StoreDTO dto)
        {
            var userType = this.GetCurrentUserUserType(_warehouseStockService.DataContext());
            if (userType != UserTypeEnum.Administrator)
            {
                return BadRequest();
            }

            var obj = _mapper.Map<Store>(dto);
            _service.InsertStore(obj);
            return Ok(obj);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _service.GetAllStores();

            var obj = _mapper.Map<IList<StoreDTO>>(list);
            return Ok(obj);
        }


        [HttpGet("{id}")]
        public IActionResult GetById(int? id)
        {
            var obj = _service.GetStoreById(id);

            if (obj == null)
            {
                return BadRequest();
            }

            var dto = _mapper.Map<StoreDTO>(obj);
            return Ok(dto);
        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]StoreDTO dto)
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

            var obj = _mapper.Map<Store>(dto);
            if (obj == null)
            {
                return BadRequest();
            }

            _service.UpdateStore(obj);
            return Ok(obj);
        }

    }
}