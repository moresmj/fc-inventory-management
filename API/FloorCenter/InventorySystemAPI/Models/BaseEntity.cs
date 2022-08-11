using System;

namespace InventorySystemAPI.Models
{
    /// <summary>
    /// All model classes must implement this class
    /// </summary>
    public abstract class BaseEntity
    {

        public abstract int Id { get; set; }

        public virtual DateTime DateCreated { get; set; }

        public virtual DateTime? DateUpdated { get; set; }

    }
}
