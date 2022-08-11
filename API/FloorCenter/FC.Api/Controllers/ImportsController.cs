using AutoMapper;
using FC.Api.DTOs.Store.PhysicalCount;
using FC.Api.DTOs.Warehouse.PhysicalCount;
using FC.Api.Helpers;
using FC.Api.Helpers.Notification;
using FC.Api.Services.Stores.PhysicalCount;
using FC.Api.Validators.Store.PhysicalCount;
using FC.Api.Validators.Warehouse.PhysicalCount;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Users;
using FC.Core.Domain.Warehouses;
using Microsoft.AspNetCore.Mvc;

namespace FC.Api.Controllers
{
    [Route("api/imports")]
    public class ImportsController : BaseController
    {

        private Services.Warehouses.PhysicalCount.IUploadPhysicalCountService _warehouseUploadPhysicalCountService;
        private Services.Warehouses.IWHStockService _whStockService;
        private IMapper _mapper;
        private Services.Stores.PhysicalCount.IUploadPhysicalCountService _storeUploadPhysicalCountService;
        private Services.Stores.ISTStockService _stStockService;

        public ImportsController(Services.Warehouses.PhysicalCount.IUploadPhysicalCountService warehouseUploadPhysicalCountService,
                                Services.Warehouses.IWHStockService whStockService,
                                IMapper mapper,
                                Services.Stores.PhysicalCount.IUploadPhysicalCountService storeUploadPhysicalCountService,
                                Services.Stores.ISTStockService stStockService)
        {
            _warehouseUploadPhysicalCountService = warehouseUploadPhysicalCountService;
            _whStockService = whStockService;
            _mapper = mapper;
            _stStockService = stStockService;
            _storeUploadPhysicalCountService = storeUploadPhysicalCountService;
        }

        #region Warehouse
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPost]
        [Route("physicalcount/warehouse")]
        public IActionResult UploadWarehousePhysicalCount([FromBody]UploadWarehousePhysicalCountDTO dto)
        {
            dto.WarehouseId = this.GetCurrentUserWarehouseId(_warehouseUploadPhysicalCountService.DataContext());
            if (!dto.WarehouseId.HasValue)
            {
                return Unauthorized();
            }

            foreach (var item in dto.Details)
            {
                item.WarehouseId = dto.WarehouseId;
            }

            var validator = new UploadWarehousePhysicalCountDTOValidator(_warehouseUploadPhysicalCountService.DataContext());

            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                return BadRequest(GetErrorMessages(results));
            }

            var mappedDto = _mapper.Map<WHImport>(dto);
            mappedDto.PhysicalCountType = PhysicalCountTypeEnum.Upload;
            var TransactionNo = _warehouseUploadPhysicalCountService.SavePhysicalCount2(mappedDto);

            var pCountDetails = _warehouseUploadPhysicalCountService.GetByTransNo(TransactionNo);

            this.ApproveWarehousePhysicalCount(pCountDetails.Id, pCountDetails);

            return Ok(TransactionNo);
        }

        [HttpPost]
        [Route("physicalcount/register/warehouse")]
        public IActionResult RegisterWarehousePhysicalCount([FromBody]UploadWarehousePhysicalCountDTO dto)
        {
            dto.WarehouseId = this.GetCurrentUserWarehouseId(_warehouseUploadPhysicalCountService.DataContext());
            if (!dto.WarehouseId.HasValue)
            {
                return Unauthorized();
            }

            foreach (var item in dto.Details)
            {
                item.WarehouseId = dto.WarehouseId;
            }

            var validator = new UploadWarehousePhysicalCountDTOValidator(_warehouseUploadPhysicalCountService.DataContext());

            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                return BadRequest(GetErrorMessages(results));
            }

            var mappedDto = _mapper.Map<WHImport>(dto);
            mappedDto.PhysicalCountType = PhysicalCountTypeEnum.Registration;
            var transNo = _warehouseUploadPhysicalCountService.SavePhysicalCount2(mappedDto);

            var pCountDetails = _warehouseUploadPhysicalCountService.GetByTransNo(transNo);

            this.ApproveWarehousePhysicalCount(pCountDetails.Id, pCountDetails);

            return Ok();
        }

        [HttpPost]
        [Route("physicalcount/breakage/warehouse")]
        public IActionResult RegisterWarehouseBreakage([FromBody]UploadWarehouseBreakageDTO dto)
        {
            dto.WarehouseId = this.GetCurrentUserWarehouseId(_warehouseUploadPhysicalCountService.DataContext());
            if (!dto.WarehouseId.HasValue)
            {
                return Unauthorized();
            }

            foreach (var item in dto.Details)
            {
                item.WarehouseId = dto.WarehouseId;
            }

            var validator = new UploadWarehouseBreakageDTOValidator(_warehouseUploadPhysicalCountService.DataContext());

            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                return BadRequest(GetErrorMessages(results));
            }

            var mappedDto = _mapper.Map<WHImport>(dto);
            mappedDto.PhysicalCountType = PhysicalCountTypeEnum.Breakage;
            var transNo = _warehouseUploadPhysicalCountService.SavePhysicalCount3(mappedDto);

            var pCountDetails = _warehouseUploadPhysicalCountService.GetByTransNo(transNo);

            this.ApproveWarehousePhysicalCount(pCountDetails.Id, pCountDetails);

            return Ok();
        }




        [Route("physicalcount/warehouse")]
        public IActionResult GetWarehousePhysicalCount_Pending()
        {
            var list = _warehouseUploadPhysicalCountService.GetAll();
            return Ok(list);
        }

        [Route("physicalcount/warehouse/{id}")]
        public IActionResult GetWarehousePhysicalCount_Pending_ById(int id)
        {
            var obj = _warehouseUploadPhysicalCountService.GetById(id);
            if(obj == null)
            {
                return BadRequest();
            }

            return Ok(obj);
        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut]
        [Route("physicalcount/warehouse/{id}")]
        public IActionResult ApproveWarehousePhysicalCount(int id, [FromBody]ApproveWarehousePhysicalCountDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            _warehouseUploadPhysicalCountService.Approve(dto, _whStockService);
            return Ok(dto);
        }


        [HttpPost]
        [Route("adjustreserve/warehouse")]
        public IActionResult AdjustReserveQuantity([FromBody]AdjustReservedItemQuantity record)
        {

            var WarehouseId = this.GetCurrentUserWarehouseId(_warehouseUploadPhysicalCountService.DataContext());
            if (!WarehouseId.HasValue)
            {
                return Unauthorized();
            }
            record.WarehouseId = WarehouseId;

            foreach (var item in record.Details)
            {
                item.WarehouseId = record.WarehouseId;
            }

            var validator = new AdjustReservedItemQuantityValidator(_warehouseUploadPhysicalCountService.DataContext());


            var results = validator.Validate(record);
            if (!results.IsValid)
            {
                return BadRequest(GetErrorMessages(results));
            }


            _warehouseUploadPhysicalCountService.SaveReserveAdjustment(record, _whStockService);


            return Ok();


        }


        #endregion

        #region Store
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPost]
        [Route("physicalcount/store")]
        public IActionResult UploadStorePhysicalCount([FromBody]UploadStorePhysicalCountDTO dto)
        {
            dto.StoreId = this.GetCurrentUserStoreId(_storeUploadPhysicalCountService.DataContext());
            if (!dto.StoreId.HasValue)
            {
                return Unauthorized();
            }

            foreach (var item in dto.Details)
            {
                item.StoreId = dto.StoreId;
            }

            var validator = new UploadStorePhysicalCountDTOValidator(_storeUploadPhysicalCountService.DataContext());

            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                return BadRequest(GetErrorMessages(results));
            }

            var mappedDto = _mapper.Map<STImport>(dto);
            mappedDto.PhysicalCountType = PhysicalCountTypeEnum.Upload;
            _storeUploadPhysicalCountService.SavePhysicalCount(mappedDto);

            return Ok(mappedDto);
        }
        [NotifyActionFilter(groupNum: AssignmentEnum.MainOffice)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPost]
        [Route("physicalcount/register/store")]
        public IActionResult RegisterStorePhysicalCount([FromBody]UploadStorePhysicalCountDTO dto)
        {
            dto.StoreId = this.GetCurrentUserStoreId(_storeUploadPhysicalCountService.DataContext());
            if (!dto.StoreId.HasValue)
            {
                return Unauthorized();
            }

            foreach (var item in dto.Details)
            {
                item.StoreId = dto.StoreId;
            }

            var validator = new UploadStorePhysicalCountDTOValidator(_storeUploadPhysicalCountService.DataContext());

            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                return BadRequest(GetErrorMessages(results));
            }

            var mappedDto = _mapper.Map<STImport>(dto);
            mappedDto.PhysicalCountType = PhysicalCountTypeEnum.Registration;
            _storeUploadPhysicalCountService.SavePhysicalCount(mappedDto);

            return Ok(mappedDto);
        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPost]
        [Route("physicalcount/breakage/store")]
        public IActionResult RegisterStoreBreakage([FromBody]RegisterStoreBreakageDTO dto)
        {
            dto.StoreId = this.GetCurrentUserStoreId(_storeUploadPhysicalCountService.DataContext());
            if (!dto.StoreId.HasValue)
            {
                return Unauthorized();
            }

            foreach (var item in dto.Details)
            {
                item.StoreId = dto.StoreId;
            }

            var validator = new RegisterBreakageCountDTOValidator(_storeUploadPhysicalCountService.DataContext());

            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                return BadRequest(GetErrorMessages(results));
            }

            var mappedDto = _mapper.Map<STImport>(dto);
            mappedDto.PhysicalCountType = PhysicalCountTypeEnum.Breakage;
            var transNo =_storeUploadPhysicalCountService.SaveBreakage(mappedDto);

            var pBreakageDetails = _storeUploadPhysicalCountService.GetByTransNo(transNo);
            _storeUploadPhysicalCountService.ApproveBreakage(pBreakageDetails, _stStockService);

            return Ok(mappedDto);
        }

        [Route("physicalcount/store")]
        public IActionResult GetStorePhysicalCount_Pending(ApprovePhysicalCountSearchDTO search)
        {
            var list = _storeUploadPhysicalCountService.GetAll(search);
            return Ok(list);
        }

       
        [Route("physicalcount/store/{id}")]
        public IActionResult GetStorePhysicalCount_Pending_ById(int id)
        {
            var obj = _storeUploadPhysicalCountService.GetById(id);
            if (obj == null)
            {
                return BadRequest();
            }

            return Ok(obj);
        }


        [NotifyActionFilter(groupNum: AssignmentEnum.MainOffice)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut]
        [Route("physicalcount/store/{id}")]
        public IActionResult ApproveStorePhysicalCount(int id, [FromBody]ApproveStorePhysicalCountDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            _storeUploadPhysicalCountService.Approve(dto, _stStockService);
            return Ok(dto);
        }

        #endregion

    }
}