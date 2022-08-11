using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Api.Services.Stores;
using FC.Core.Domain.Stores;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators
{
    public class AddTransferOrderStoreItemsValidator : AbstractValidator<AddTransferOrderStoreItemsDTO>
    {
        private readonly DataContext _context;
        private ISTStockService _stockService;


        public AddTransferOrderStoreItemsValidator(DataContext context, ISTStockService stockService)
        {
            
            this._context = context;
            _stockService = stockService;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate ItemId
            RuleFor(p => p)
                    .NotEmpty()
                    .Must(IsItemAvailableOnStore)
                        .WithMessage(p => string.Format("[{0}] Item is not available on store [{1}]", GetItemCode(p.ItemId), GetStoreName(p.StoreId)));

            RuleFor(p => p)
                    .NotEmpty()
                    .Must(IsQtyMustNotBEGreaterthanAvailable)
                        .WithMessage(p => string.Format("[{0}] Ordered quantity should not be greater than Available quantity on store [{1}]", GetItemCode(p.ItemId), GetStoreName(p.StoreId)));


        }

        private bool IsItemAvailableOnStore(AddTransferOrderStoreItemsDTO details)
        {
            _stockService.stOrder = _context.STOrders;
            _stockService.stStock = _context.STStocks;
            _stockService.stSales = _context.STSales;
            var available = _stockService.GetItemAvailableQuantity(details.ItemId, details.StoreId, true);

            if (available <= 0)
            {
                return false;
            }
            return true;
        }

        private bool IsQtyMustNotBEGreaterthanAvailable(AddTransferOrderStoreItemsDTO details)
        {
            _stockService.stOrder = _context.STOrders;
            _stockService.stStock = _context.STStocks;
            _stockService.stSales = _context.STSales;
            var available = _stockService.GetItemAvailableQuantity(details.ItemId, details.StoreId, true);

            if (details.Quantity > available)
            {
                return false;
            }
            return true;
        }

        private string GetItemCode(int? itemId)
        {
            return _context.Items.Where(p => p.Id == itemId).Select(p => p.Code).FirstOrDefault();
        }

        private string GetStoreName(int? storeId)
        {
            return _context.Stores.Where(p => p.Id == storeId).Select(p => p.Name).FirstOrDefault();
        }



    }


}

