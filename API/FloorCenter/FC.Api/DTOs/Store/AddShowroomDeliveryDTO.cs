using FC.Api.Validators.Store;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;

namespace FC.Api.DTOs.Store
{

    [Validator(typeof(AddShowroomDeliveryDTOValidator))]
    public class AddShowroomDeliveryDTO
    {

        /// <summary>
        /// STOrder.Id
        /// </summary>
        public int Id { get; set; }

        public string DRNumber { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public string Remarks { get; set; }

        public ICollection<AddShowroomDeliveryItems> ShowroomDeliveries { get; set; }

    }
}
