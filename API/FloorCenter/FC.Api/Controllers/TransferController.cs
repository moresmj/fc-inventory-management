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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;

namespace FC.Api.Controllers
{
    [Route("api/transfer")]
    public class TransferController : BaseController
    {

        private ISTTransferService _transferService;
        private ISTStockService _stockService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public TransferController(ISTTransferService transferService, ISTStockService stockService, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _transferService = transferService;
            _stockService = stockService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }
        //[ServiceFilter(typeof(UserTrailActionFilter))]
        [NotifyActionFilter(groupNum: AssignmentEnum.MainOffice)]
        [HttpPost]
        public IActionResult AddTransferOrder([FromBody]AddTransferOrderDTO dto)
        {
            dto.StoreId = this.GetCurrentUserStoreId(_transferService.DataContext());
            dto.TRNumber = _transferService.GetTransactionNumberForMultipleStore();

            if (!dto.StoreId.HasValue)
            {
                return Unauthorized();
            }

            foreach (var order in dto.TransferOrders)
            {

                var obj = _mapper.Map<STTransfer>(order);

                dto.TransferId = _transferService.AddTransferOrder(obj, _stockService);
                dto.OrderToStoreId = order.StoreId;

                if (dto.TransferId.HasValue)
                {
                    foreach (var orderDetails in order.TransferredItems)
                    {
                        orderDetails.STTransferId = dto.TransferId;
                    }
                    var validator = new AddTransferOrderDTOValidator(_transferService.DataContext(), _stockService);
                    _transferService.SaveTransferToOrder(dto, this.currentUser, _appSettings);
                }

            }

            return Ok(dto);
        }


    }
}