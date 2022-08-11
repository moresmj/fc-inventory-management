using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Api.Services.Stores;
using FC.Api.Services.Stores.Returns;
using FC.Api.Services.Warehouses;
using FC.Core.Domain.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FC.Api.Controllers
{
  
    [Route("api/transactions/approverequestsV2")]
    public class ApproveRequestsV2Controller : BaseController
    {

        private ISTStockService _stockService;
        private ISTSalesService _salesService;
        private ISTOrderService _service;
        private IWHStockService _whStockService;
        private IReturnService _returnService;
        private ISTTransferService _transferService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;



        public ApproveRequestsV2Controller(
            ISTStockService stockService,
            ISTSalesService salesService,
            IWHStockService whStockService,
            ISTOrderService service,
            IReturnService returnService,
            ISTTransferService transferService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _stockService = stockService;
            _salesService = salesService;
            _service = service;
            _whStockService = whStockService;
            _returnService = returnService;
            _transferService = transferService;
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


            //var list = _service.GetAllOrders3(search, _appSettings);
            //list
            return Ok();



        }
    }
}