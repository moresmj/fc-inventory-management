using FC.Api.Helpers;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace FC.Api.Services.Users
{
    public interface IUserService
    {

        DataContext DataContext();

        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="password">Password</param>
        /// <returns>User</returns>
        User Authenticate(string userName, string password);

        List<Dictionary<string, object>> GetStoresHandled(int? UserId);
        /// <summary>
        /// Insert a user
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="password">Password</param>
        void InsertUser(User user, string password);

        void InsertUserDealer(User user, string password, IList<int> storesHandled);

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>User List</returns>
        IEnumerable<User> GetAllUsers();

        IEnumerable<UserDealer> GetAllUserDealer();

        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>User</returns>
        User GetUserById(int? id);


        /// <summary>
        /// Updates the user
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="password">Password</param>
        void UpdateUser(User user, string password = null);
        
        void UpdateUserStatus(int? userId);

        void UpdateUserDealer(User user, IList<int> storesHandled, string password);

        IEnumerable<User> GetAllUsersByWarehouseId(int? warehouseId);

    }
}
