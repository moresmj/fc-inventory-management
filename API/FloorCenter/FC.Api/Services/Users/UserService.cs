using FC.Api.Helpers;
using FC.Api.Services.Stores;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FC.Api.Services.Users
{
    public class UserService : IUserService
    {

        private DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }


        public DataContext DataContext()
        {
            return _context;
        }

        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="password">Password</param>
        /// <returns>User</returns>
        public User Authenticate(string userName, string password)
        {
            //  Check if username or password is null or empty
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var user = _context.Users
                                 .Include(p => p.Store)
                                 .Include(p => p.Warehouse)
                                 .SingleOrDefault(x => x.UserName == userName && x.IsActive == true);

            // Check if username exists
            if (user == null)
            {
                return null;
            }

            // Check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            // Authentication successful
            return user;
        }

        public List<Dictionary<string, object>> GetStoresHandled(int? UserId)
        {
            var assignments = _context.StoreDealerAssignment
                                 .Include(p => p.Store).Where(x => x.UserId == UserId);

            var storeList = new List<Dictionary<string, object>>();
            foreach (var assign in assignments)
            {
                var dic = new Dictionary<string, object>();
                dic.Add("id", assign.StoreId);
                dic.Add("name", assign.Store.Name);
                dic.Add("store", assign.Store);

                storeList.Add(dic);
            }

            return storeList;

        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            }

            if (storedHash.Length != 64)
            {
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            }

            if (storedSalt.Length != 128)
            {
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");
            }

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Insert a user
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="password">Password</param>
        public void InsertUser(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            if (user.Assignment != AssignmentEnum.Warehouse)
            {
                user.WarehouseId = null;
            }

            if (user.Assignment != AssignmentEnum.Store)
            {
                user.StoreId = null;
            }

            user.DateCreated = DateTime.Now;

            _context.Users.Add(user);
            _context.SaveChanges();
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            }

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>User List</returns>
        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.Where(x => x.UserType != UserTypeEnum.Dealer).AsNoTracking().OrderByDescending(p => p.Id);
        }

        public IEnumerable<UserDealer> GetAllUserDealer()
        {
            var stores = _context.Stores.AsNoTracking().OrderByDescending(p => p.Id);
            var users = _context.Users.Where(x => x.UserType == UserTypeEnum.Dealer).AsNoTracking().OrderByDescending(p => p.Id).ToList();

            var list = new List<UserDealer>();

            for (int i = 0; i < users.Count(); i++)
            {             
                var handled = _context.StoreDealerAssignment.Where(x => x.UserId == users[i].Id).Include(x => x.Store).AsNoTracking().Select(x => x.Store).ToList();

                var Dealer = new UserDealer();
                Dealer.Id = users[i].Id;
                Dealer.UserName = users[i].UserName;
                Dealer.FullName = users[i].FullName;
                Dealer.EmailAddress = users[i].EmailAddress;
                Dealer.ContactNumber = users[i].ContactNumber;
                Dealer.Address = users[i].Address;
                Dealer.IsActive = users[i].IsActive;
                Dealer.LastLogin = users[i].LastLogin;
                Dealer.UserType = users[i].UserType;
                Dealer.Handled = handled;
                list.Add(Dealer);
            } 
            return list;
        }


        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>User</returns>
        public User GetUserById(int? id)
        {
            return _context.Users.Find(id);
        }

        /// <summary>
        /// Updates the user
        /// </summary>
        /// <param name="userParam">User</param>
        /// <param name="password">Password</param>
        public void UpdateUser(User userParam, string password = null)
        {
            var user = _context.Users.Where(x => x.Id == userParam.Id).SingleOrDefault();

            user.FullName = userParam.FullName;
            user.EmailAddress = userParam.EmailAddress;
            user.Address = userParam.Address;
            user.ContactNumber = userParam.ContactNumber;
            user.Assignment = userParam.Assignment;
            user.UserType = userParam.UserType;
            //user.IsActive = userParam.IsActive;

            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            if (userParam.Assignment == AssignmentEnum.Warehouse)
            {
                user.WarehouseId = userParam.WarehouseId;
            }
            else
            {
                user.WarehouseId = null;
            }

            if (userParam.Assignment == AssignmentEnum.Store)
            {
                user.StoreId = userParam.StoreId;
            }
            else
            {
                user.StoreId = null;
            }

            user.DateUpdated = DateTime.Now;

            _context.Users.Update(user);
            _context.SaveChanges();
        }



        public void UpdateUserStatus(int? userId)
        {

            var user = _context.Users.Where(p => p.Id == userId).FirstOrDefault();
            if(user != null)
            {
                user.IsActive = user.IsActive == null ? true : !user.IsActive;

                _context.Users.Update(user);
                _context.SaveChanges();
            }

            

        }

        #region Dealer


        public void InsertUserDealer(User user, string password, IList<int> storesHandled)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            user.UserType = UserTypeEnum.Dealer;
            user.Assignment = AssignmentEnum.Store;

            user.DateCreated = DateTime.Now;

            user.IsActive = true;

            _context.Users.Add(user);
            _context.SaveChanges();

            for (int i = 0; i < storesHandled.Count; i++)
            {


                var assign = new StoreDealerAssignment
                {
                    UserId = user.Id,
                    StoreId = storesHandled[i],
                    DateCreated = DateTime.Now,
                    User = null,
                    Store = null
                };


                new StoreService(_context).InsertStoresHandled(assign);
            };
        }

        public void UpdateUserDealer(User userParam, IList<int> storesHandled, string password = null)
        {
            var user = _context.Users.Where(x => x.Id == userParam.Id).SingleOrDefault();

            user.FullName = userParam.FullName;
            user.EmailAddress = userParam.EmailAddress;
            user.Address = userParam.Address;
            user.ContactNumber = userParam.ContactNumber;


            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            user.DateUpdated = DateTime.Now;

            _context.Users.Update(user);
            _context.SaveChanges();

            var storeService = new StoreService(_context);
            for (int i = 0; i < storesHandled.Count; i++)
            {
                if (i == 0)
                {
                    var currentStoreHandled = _context.StoreDealerAssignment.Where(x => x.UserId == user.Id).ToList();
                    _context.StoreDealerAssignment.RemoveRange(currentStoreHandled);
                    _context.SaveChanges();
                }

                var assign = new StoreDealerAssignment {
                    UserId = user.Id,
                    StoreId = storesHandled[i],
                    DateCreated = DateTime.Now
                };

                new StoreService(_context).InsertStoresHandled(assign);
            }   
        }


        #endregion

        public IEnumerable<User> GetAllUsersByWarehouseId(int? warehouseId)
        {
            return _context.Users
                        .Where(p => p.Assignment == AssignmentEnum.Warehouse
                                    && p.WarehouseId == warehouseId);
        }

    }
}
