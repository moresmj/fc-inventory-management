using System;

namespace FC.Batch.DTOs
{

    /// <summary>
    /// Base class for entities
    /// </summary>
    public abstract partial class BaseEntity
    {
        public int Id { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
    }

}
