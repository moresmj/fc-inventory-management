using AutoMapper;
using FC.Api.DTOs.Store.BranchOrders;
using FC.Api.Helpers;
using FC.Api.Helpers.Notification;
using FC.Api.Services.Stores;
using FC.Api.Validators.Store.BranchOrders;
using FC.Core.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FC.Api.Controllers
{
    [Route("api/branchorders")]
    public class BranchOrdersController : BaseController
    {

        private IBranchOrdersService _service;
        private ISTStockService _stockService;
        private ISTSalesService _salesService;
        private IMapper _mapper;
        private readonly AppSettings _appSetting;

        public BranchOrdersController(IBranchOrdersService service, 
            ISTStockService stockService, 
            ISTSalesService salesService, 
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _service = service;
            _stockService = stockService;
            _salesService = salesService;
            _mapper = mapper;
            _appSetting = appSettings.Value;
        }

        [HttpGet]
        public IActionResult GetBranchOrders(Search search)
        {
            search.OrderToStoreId = this.GetCurrentUserStoreId(_service.DataContext());
            //var list = _service.GetAllBranchOrders(search, _stockService);
            var list = _service.GetAllBranchOrdersPaged(search, _stockService, _appSetting);

            return Ok(list);
        }

        [NotifyActionFilter(groupNum: AssignmentEnum.Store)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut("{id}")]
        public IActionResult UpdateTransferRequest(int id, [FromBody]UpdateTransferRequestDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            dto.OrderToStoreId = this.GetCurrentUserStoreId(_service.DataContext());
            if (!dto.OrderToStoreId.HasValue)
            {
                return Unauthorized();
            }

            var validator = new UpdateTransferRequestDTOValidator(_service.DataContext());

            var results = validator.Validate(dto);
            if (!results.IsValid)
            {
                return BadRequest(GetErrorMessages(results));
            }
            
            _service.UpdateTransferRequest(dto, _salesService);

            return Ok(dto);
        }
        
    }
}