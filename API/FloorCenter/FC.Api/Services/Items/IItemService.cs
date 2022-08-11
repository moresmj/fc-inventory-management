using FC.Api.DTOs.Item;
using FC.Api.Helpers;
using FC.Core.Domain.Items;
using System.Collections.Generic;

namespace FC.Api.Services.Items
{
    public interface IItemService
    {

        /// <summary>
        /// Insert an item
        /// </summary>
        /// <param name="item">Item</param>
        void InsertItem(Item item);


        /// <summary>
        /// Get all items
        /// </summary>
        /// <param name="dto">Search parameters</param>
        /// <returns>Items</returns>
        IEnumerable<Item> GetAllItems(ItemSearchDTO dto);


        /// <summary>
        /// Updates the item
        /// </summary>
        /// <param name="itemParam">Item</param>
        void UpdateItem(Item itemParam);


        List<Item> GetAllItemsForDropDown();


        object GetAllItems2(ItemSearchDTO dto, AppSettings appSettings);


    }
}
