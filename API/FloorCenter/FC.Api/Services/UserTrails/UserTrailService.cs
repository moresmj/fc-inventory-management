using FC.Api.DTOs.Usertrail;
using FC.Api.Helpers;
using FC.Api.Services.Users;
using FC.Core.Domain.Users;
using FC.Core.Domain.UserTrail;
using FC.Core.Helper.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.UserTrails
{
    public class UserTrailService : IUserTrailService
    {

        private DataContext _context;
        

        public UserTrailService(DataContext context)
        {

            _context = context;

        }

        public void InsertUserLoginTrail(IUserService _userService, User user)
        {
            var AssignedAt = "";

            if (user.Assignment == AssignmentEnum.Store)
            {
                AssignedAt = _context.Stores.AsNoTracking().Where(p => p.Id == user.StoreId).Select(p => p.Code).FirstOrDefault();
            }
            else if (user.Assignment == AssignmentEnum.Warehouse)
            {
                AssignedAt = _context.Warehouses.AsNoTracking().Where(p => p.Id == user.WarehouseId).Select(p => p.Code).FirstOrDefault();
            }
            else if (user.Assignment == AssignmentEnum.MainOffice)
            {
                AssignedAt = "Main Office";
            }
            else if (user.Assignment == AssignmentEnum.Logistics)
            {
                AssignedAt = "Logistics";
            }

            var trailDetails = new UserTrail();
            trailDetails.DateCreated = DateTime.Now;
            trailDetails.Action = "Authenticate";
            trailDetails.UserId = user.Id;
            trailDetails.Transaction = "Login";
            trailDetails.Detail = "User login's at " + AssignedAt;

            _context.UserTrails.Add(trailDetails);
            _context.SaveChanges();


            // Update LastLogin
            user.LastLogin = DateTime.Now;

            _userService.UpdateUser(user);
            _context.SaveChanges();

        }


        public void InsertUserTrail(string Action, int UserId, string transaction, string detail)
        {

            var trailDetails = new UserTrail();
            trailDetails.Action = Action;
            trailDetails.UserId = UserId;
            trailDetails.Transaction = transaction;
            trailDetails.Detail = detail;

            trailDetails.DateCreated = DateTime.Now;


            _context.UserTrails.Add(trailDetails);
            _context.SaveChanges();

        }


        public void InsertTrail(UserTrail userTrail)
        {
            _context.UserTrails.Add(userTrail);
            _context.SaveChanges();
        }

        public object GetAllUserTrail(SearchUserTrail search, AppSettings appSettings)
        {
            IQueryable<UserTrail> query = _context.UserTrails;


            if(search.UserID.HasValue)
            {
                query = query.Where(p => p.UserId == search.UserID);
            }

            if (search.DateFrom.HasValue)
            {
                    query = query.Where(p => search.DateFrom.Value <= p.DateCreated);
            }

            if (search.DateTo.HasValue)
            {
                    search.DateTo = search.DateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    query = query.Where(p => search.DateTo.Value >= p.DateCreated);
            }

            GetAllResponse response = null;
            query = query.OrderByDescending(p => p.Id);

            if (search.ShowAll == false)
            {
                response = new GetAllResponse(query.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


                //check if currentpage is greater than totalpage
                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                    error.ErrorMessages.Add(MessageHelper.NoRecordFound);

                    return error;


                }

                query = query.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                            .Take(appSettings.RecordDisplayPerPage);



            }
            else
            {
                response = new GetAllResponse(query.Count());
            }

            


            var records = from x in query
                          select new
                          {
                              x.Id,
                              x.Transaction,
                              x.Detail,
                              x.DateCreated,
                              x.Action,
                              UserName = _context.Users.Where(p => p.Id == x.UserId).Select(z => z.UserName).FirstOrDefault()

                          };


            response.List.AddRange(records);


            return response;
        }
    }
}
