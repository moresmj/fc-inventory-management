using FC.Api.DTOs.User;
using FC.Api.Validators.Warehouse;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;

namespace FC.Api.DTOs.Warehouse
{

    [Validator(typeof(WHReceiveDTOValidator))]
    public class WHReceiveDTO
    {

        public int Id { get; set; }

        public string TransactionNo { get; set; }

        public int? WarehouseId { get; set; }

        public string PONumber { get; set; }

        public DateTime? PODate { get; set; }

        public string DRNumber { get; set; }

        public DateTime? DRDate { get; set; }

        public DateTime? ReceivedDate { get; set; }

        public string Remarks { get; set; }

        public int? UserId { get; set; }

        public ICollection<WHReceiveDetailDTO> ReceivedItems { get; set; }

        public UserDTO User { get; set; }

    }
}
