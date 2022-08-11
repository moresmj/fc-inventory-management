using FC.Api.Validators.Store;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;

namespace FC.Api.DTOs.Store
{

    [Validator(typeof(SaveReceiveItemValidator))]
    public class SaveReceiveItem
    {

        /// <summary>
        /// STDelivery.Id
        /// </summary>
        public int Id { get; set; }

        public ICollection<SaveReceiveItemDeliveries> ShowroomDeliveries { get; set; }

        public string DRNumber { get; set; }

        public DateTime? DeliveryDate { get; set; }

    }
}
