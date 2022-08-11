using System.ComponentModel.DataAnnotations;

namespace FC.Core.Domain.UserTrail
{
  public class UserTrail : BaseEntity
    {

        [MaxLength(255)]
        public string Action { get; set; }

        public int UserId { get; set; }

        [MaxLength(255)]
        public string Transaction { get; set; }

        public string Detail { get; set; }
    }
}
