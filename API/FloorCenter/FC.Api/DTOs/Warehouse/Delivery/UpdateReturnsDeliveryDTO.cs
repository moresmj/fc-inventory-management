using FC.Api.Validators.Warehouse.Delivery;
using FC.Core.Domain.Common;
using FluentValidation.Attributes;
using System;

namespace FC.Api.DTOs.Warehouse.Delivery
{

    [Validator(typeof(UpdateReturnsDeliveryDTOValidator))]
    public class UpdateReturnsDeliveryDTO
    {

        public int Id { get; set; }

        public DateTime? ApprovedDeliveryDate { get; set; }

        public string DriverName { get; set; }

        public string PlateNumber { get; set; }

        public DeliveryStatusEnum? DeliveryStatus { get; set; }

        public string TransactionNo { get; set; }

    }
}
