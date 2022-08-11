using FC.Api.Helpers;
using FC.Core.Domain.Users;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace FC.Api.Controllers
{
    [Authorize]
    [Produces("application/json")]
    public abstract class BaseController : Controller
    {
        
        protected ClaimsPrincipal currentUser;
        protected int? storeId;
        private Regex regex = new Regex(@"[\d]");

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            this.currentUser = context.HttpContext.User;

            this.SetCurrentUserStoreId(context);
        }

        protected void SetCurrentUserStoreId(ActionExecutingContext context)
        {
            int storeIdHeader = context.HttpContext.Request.Headers.Where(x => x.Key == "storeId").Count();
            if (storeIdHeader > 0)
            {
                var rawStoreId = context.HttpContext.Request.Headers.Where(x => x.Key == "storeId").Select(x => x.Value).ToList();
                if (regex.IsMatch(rawStoreId[0]))
                {
                    this.storeId = Convert.ToInt16(rawStoreId[0]);
                }
                else
                {
                    this.storeId = null;
                }
            }
        }


        protected int? GetCurrentUserWarehouseId(DataContext dataContext)
        {

            int? id = null;

            var user = this.GetUser(dataContext);
            if (user != null)
            {
                id = user.WarehouseId;
            }

            return id;
        }

        protected int? GetCurrentUserStoreId(DataContext dataContext)
        {

            int? id = null;
            if (this.storeId.HasValue)
            {
                this.ValidateDealerStoreId(dataContext);
                return this.storeId;
            }

            var user = this.GetUser(dataContext);
            if (user != null)
            {
                id = user.StoreId;
            }

            return id;
        }

        protected UserTypeEnum? GetCurrentUserUserType(DataContext dataContext)
        {

            UserTypeEnum? userType = null;

            var user = this.GetUser(dataContext);
            if (user != null)
            {
                userType = user.UserType;
            }

            return userType;
        }

        protected User GetUser(DataContext dataContext)
        {
            User retUser = null;

            if (currentUser != null)
            {
                foreach (var c in currentUser.Claims)
                {
                    if (c.Type == JwtRegisteredClaimNames.Sub)
                    {
                        var userName = c.Value;
                        if (userName != null)
                        {
                            retUser = dataContext.Users.Where(p => p.UserName.ToLower() == userName).FirstOrDefault();
                            if (retUser != null)
                            {
                                break;
                            }
                        }
                    }
                    else if (c.Type == JwtRegisteredClaimNames.Jti)
                    {
                        var id = Convert.ToInt32(c.Value);
                        if (id != 0)
                        {
                            retUser = dataContext.Users.Where(p => p.Id == id).FirstOrDefault();
                            if (retUser != null)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return retUser;
        }

        protected Exception ValidateDealerStoreId(DataContext context)
        {
            var store = context.StoreDealerAssignment.Where(x => x.StoreId == this.storeId);
            if(store == null)
            {
                throw new Exception("ERROR00001");
            }
            return null;
        }

        protected Dictionary<string, object> GetErrorMessages(ValidationResult results)
        {
            var allErrors = new Dictionary<string, object>();
            var errors = new List<string>();

            string prevPropName = "";
            for (int i = 0; i < results.Errors.Count; i++)
            {
                if (prevPropName == "")
                {
                    prevPropName = results.Errors[i].PropertyName;
                }


                if (prevPropName.ToLower() != results.Errors[i].PropertyName.ToLower())
                {
                    allErrors.Add(prevPropName, errors);

                    prevPropName = results.Errors[i].PropertyName;

                    errors = new List<string>();
                }

                errors.Add(results.Errors[i].ErrorMessage);
                if ((i + 1) == results.Errors.Count)
                {
                    allErrors.Add(prevPropName, errors);
                }

            }

            return allErrors;
        }

    }
}