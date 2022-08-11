using AutoMapper;
using FC.Api.DTOs.Store;
using FC.Api.DTOs.Store.AdvanceOrder;
using FC.Api.DTOs.Store.BranchOrders;
using FC.Api.DTOs.Store.Returns;
using FC.Api.DTOs.Store.Transfers;
using FC.Api.DTOs.Warehouse.ModifyTonality;
using FC.Api.Helpers;
using FC.Api.Helpers.Notification;
using FC.Api.Services.Stores;
using FC.Api.Services.Stores.AdvanceOrder;
using FC.Api.Services.Stores.Releasing;
using FC.Api.Services.Stores.Returns;
using FC.Api.Services.Warehouses;
using FC.Api.Services.Warehouses.ModifyTonality;
using FC.Api.Validators.Store.BranchOrders;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Users;
using FC.Core.Domain.Warehouses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FC.Api.Controllers
{

    [Route("api/transactions/approverequests")]
    public class ApproveRequestsController : BaseController
    {
        private ISTStockService _stockService;
        private ISTSalesService _salesService;
        private ISTOrderService _service;
        private IWHStockService _whStockService;
        private IReturnService _returnService;
        private ISTTransferService _transferService;
        private IModifyTonalityService _modifyTonalityService;
        private ISTAdvanceOrderService _advanceOrderService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public ApproveRequestsController(ISTStockService stockService, 
            ISTSalesService salesService, 
            IWHStockService whStockService, 
            ISTOrderService service,
            IReturnService returnService,
            ISTTransferService transferService,
            IModifyTonalityService modifyTonalityService,
            ISTAdvanceOrderService advanceOrderService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _stockService = stockService;
            _salesService = salesService;
            _service = service;
            _whStockService = whStockService;
            _returnService = returnService;
            _transferService = transferService;
            _modifyTonalityService = modifyTonalityService;
            _advanceOrderService = advanceOrderService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public IActionResult GetAllForApprovalRequests(SearchApproveRequests search)
        {
            var userType = this.GetCurrentUserUserType(_whStockService.DataContext());
            if (userType != UserTypeEnum.Administrator && userType != UserTypeEnum.MainInventory)
            {
                return BadRequest();
            }

            var list = _service.GetAllOrders3(search, _appSettings);

            return Ok(list);
        }

        [HttpGet("{id}")]
        public IActionResult GetRequestById(int? id)
        {
            var obj = _service.GetOrderById(id);
            if (obj == null)
            {
                return BadRequest();
            }

            var dto = _mapper.Map<STOrderDTO>(obj);
            return Ok(dto);
        }

        [NotifyActionFilter(groupNum: AssignmentEnum.Store)]
        [NotifyActionFilter(groupNum: AssignmentEnum.MainOffice)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut("{id}")]
        public IActionResult UpdateRequest(int id, [FromBody]UpdateRequestDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var obj = _mapper.Map<STOrder>(dto);
            _service.UpdateOrder(obj);

            return Ok(obj);
        }

        [NotifyActionFilter(groupNum: AssignmentEnum.MainOffice)]
        [Route("returns")]
        public IActionResult GetAllForApprovalRequests_Returns(SearchReturns search)
        {
            var userType = this.GetCurrentUserUserType(_whStockService.DataContext());
            if (userType != UserTypeEnum.Administrator && userType != UserTypeEnum.MainInventory)
            {
                return BadRequest();
            }
            search.mainList = true;

            //var list = _returnService.GetAllForApproval(search);
            //var list = _returnService.GetForApprovalPaged(search, _appSettings);
            var list = _returnService.GetForApprovalPaged2(search, _appSettings);

            return Ok(list);
        }

        [NotifyActionFilter(groupNum: AssignmentEnum.MainOffice)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut]
        [Route("returns/purchasereturn/{id}")]
        public IActionResult Approve_PurchaseReturn(int id)
        {
            var obj = _returnService.GetPurchaseReturnBy(id);
            if (obj == null)
            {
                return BadRequest();
            }

            _returnService.ApprovePurchaseReturn(obj);

            return Ok(obj);
        }

        [NotifyActionFilter(groupNum: AssignmentEnum.MainOffice)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut]
        [Route("returns/purchasereturncancel/{id}")]
        public IActionResult Cancel_PurchaseReturn(int id)
        {
            var obj = _returnService.GetPurchaseReturnBy(id);
            
            if (obj == null)
            {
                return BadRequest();
            }
            obj.RequestStatus = RequestStatusEnum.Cancelled;

            _returnService.ApprovePurchaseReturn(obj);

            return Ok(obj);
        }


        [Route("transfers")]
        public IActionResult GetAllForApprovalRequests_Transfers(SearchTransfers search)
        {
            var userType = this.GetCurrentUserUserType(_whStockService.DataContext());
            if (userType != UserTypeEnum.Administrator && userType != UserTypeEnum.MainInventory)
            {
                return BadRequest();
            }

            //var list = _transferService.GetAllForTransferApproval(search, _stockService);
            var list = _transferService.GetForTransferApprovalPaged2(search, _stockService, _appSettings);

            return Ok(list);
        }

        [NotifyActionFilter(groupNum: AssignmentEnum.MainOffice)]
        [NotifyActionFilter(groupNum: AssignmentEnum.Store)]
        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut("transfers/{id}")]
        public IActionResult ApproveTransferRequest(int id, [FromBody]ApproveTransferRequestDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            _transferService.ApproveTransferRequestOnMain(dto, _salesService);

            return Ok(dto);
        }

        [HttpGet]
        [Route("modifytonality")]
        public IActionResult GetAllForApprovalRequests_ModifyTonality(SearchModifyTonality search)
        {
            var list = _modifyTonalityService.GetAllForApprovalRequests_ChangeItemTonality(search, _appSettings);

            return Ok(list);
        }

        [HttpPut("modifytonality/{id}")]
        public IActionResult ApproveModifyTonalityRequest(int id, [FromBody]ApproveModifyTonalityDTO record)
        {
            if (id != record.Id)
            {
                return BadRequest();
            }
            _modifyTonalityService.ApproveModifyTonality(record);

            return Ok();
        }

        #region Advance Order
        [HttpGet]
        [Route("advanceorders")]
        public IActionResult GetAllForAdvanceOrders(SearchApproveRequests search)
        {

            var orders = _service.GetAllAdvanceOrders(search, _appSettings);

            return Ok(orders);
        }


        [HttpPut("advanceorders/{id}")]
        public IActionResult ApproveAdvanceOrder(int id, [FromBody]STAdvanceOrderDTO record)
        {
            if(id != record.Id)
            {
                return BadRequest();
            }

            _advanceOrderService.ApproveAdvanceOrder(record);

            return Ok();
        }
        #endregion


    }
}