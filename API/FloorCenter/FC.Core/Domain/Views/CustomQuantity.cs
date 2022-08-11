using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FC.Core.Domain.Views
{
    [NotMapped]
    public class CustomQuantity : BaseEntity
    {
        public int Total { get; set; }
    }
}
