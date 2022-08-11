using FC.Api.Validators.Store.Deliveries;
using FC.Core.Domain.Common;
using FluentValidation.Attributes;
using System;

namespace FC.Api.DTOs.Store.Deliveries
{
    [Validator(typeof(UpdateStoreReturnsDeliveryDTOValidator))]
    public class UpdateStoreReturnsDeliveryDTO
    {

        public int Id { get; set; }

        public DateTime? ApprovedDeliveryDate { get; set; }

        public string DriverName { get; set; }

        public string PlateNumber { get; set; }

        public DeliveryStatusEnum? DeliveryStatus { get; set; }

        public string TransactionNo { get; set; }

    }
}
