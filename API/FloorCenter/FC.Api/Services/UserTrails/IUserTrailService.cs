using FC.Api.DTOs.Usertrail;
using FC.Api.Helpers;
using FC.Api.Services.Users;
using FC.Core.Domain.Users;
using FC.Core.Domain.UserTrail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.UserTrails
{
    public interface IUserTrailService
    {

        void InsertUserTrail(string Action, int UserId, string transaction, string detail);

        void InsertTrail(UserTrail userTrail);

        object GetAllUserTrail(SearchUserTrail search, AppSettings appSettings);

        void InsertUserLoginTrail(IUserService _userService, User user);

    }
}
