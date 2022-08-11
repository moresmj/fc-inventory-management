using FluentValidation.Attributes;
using InventorySystemAPI.Validators.Store.Releasing;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.Store.Releasing
{

    [Validator(typeof(STReleaseDetailValidator))]
    public class STReleaseDetail : BaseEntity
    {

        [Column(Order = 0)]
        public override int Id { get; set; }

        [Column(Order = 1)]
        public int? STReleaseId { get; set; }

        [Column(Order = 2)]
        public int? STSalesDetailId { get; set; }

        [Column(Order = 3)]
        public int? Quantity { get; set; }
        
    }
}
