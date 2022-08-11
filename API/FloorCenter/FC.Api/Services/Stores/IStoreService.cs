using FC.Api.DTOs.User;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Users;
using System.Collections.Generic;

namespace FC.Api.Services.Stores
{
    public interface IStoreService
    {

        /// <summary>
        /// Insert a store
        /// </summary>
        /// <param name="store">Store</param>
        void InsertStore(Store store);


        /// <summary>
        /// Get all stores
        /// </summary>
        /// <returns>Store List</returns>
        IEnumerable<Store> GetAllStores();


        /// <summary>
        /// Get store by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Store</returns>
        Store GetStoreById(int? id);


        /// <summary>
        /// Updates the store
        /// </summary>
        /// <param name="store">Store</param>
        void UpdateStore(Store store);

        void InsertStoresHandled(StoreDealerAssignment assign);

    }
}
