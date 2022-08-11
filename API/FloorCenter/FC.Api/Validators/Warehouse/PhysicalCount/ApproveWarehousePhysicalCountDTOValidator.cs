using System;
using System.Linq;
using FC.Api.DTOs.Warehouse.PhysicalCount;
using FC.Api.Helpers;
using FC.Api.Services.Warehouses;
using FC.Api.Services.Warehouses.PhysicalCount;
using FluentValidation;

namespace FC.Api.Validators.Warehouse.PhysicalCount
{
    public class ApproveWarehousePhysicalCountDTOValidator : AbstractValidator<ApproveWarehousePhysicalCountDTO>
    {

        private readonly DataContext _context;
        private IWHStockService _warehouseStockService;


        public ApproveWarehousePhysicalCountDTOValidator(DataContext context, IWHStockService whStockService)
        {
            this._context = context;
            this._warehouseStockService = whStockService;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate Id
            RuleFor(p => p.Id)
                .Must(ValidRecord)
                    .WithMessage("Record is not valid")
                    .WithName("Id");

            RuleFor(p => p.Details)
                .NotEmpty()
                    .WithMessage("Please add at least one record to be approved.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one record to be approved.")
                .SetCollectionValidator(new ApproveWarehousePhysicalCountItemsValidator(context));

            RuleFor(p => p)
                .Must(IdShouldMatchListWHImportId)
                    .WithMessage("Invalid item(s) found on the list.")
                    .WithName("Id & Details.WHImportId");

        }



        private bool ValidRecord(int Id)
        {
            var service = new UploadPhysicalCountService(_context,_warehouseStockService);
            if(service.GetById(Id) == null)
            {
                return false;
            }

            return true;
        }

        private bool IdShouldMatchListWHImportId(ApproveWarehousePhysicalCountDTO model)
        {
            if (model.Details != null && model.Details.Count > 0)
            {
                return (model.Details.Where(p => p.WHImportId != model.Id).Count() == 0);
            }

            return true;
        }
    }
}
