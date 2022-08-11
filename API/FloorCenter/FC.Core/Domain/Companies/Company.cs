using System.ComponentModel.DataAnnotations;

namespace FC.Core.Domain.Companies
{
    public class Company : BaseEntity
    {
        [MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

    }
}
