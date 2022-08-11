using FC.Api.Services.UserTrails;
using FC.Core.Domain.UserTrail;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FC.Api.Helpers
{
    public class UserTrailActionFilter : ActionFilterAttribute
    {
        protected ClaimsPrincipal currentUser;
        private IUserTrailService _trailService;

        public UserTrailActionFilter(IUserTrailService trailService)
        {
            _trailService = trailService;

        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {

            this.currentUser = context.HttpContext.User;
            string id = null;
            string type = null;


            base.OnActionExecuted(context);

            var descriptor = context?.ActionDescriptor as ControllerActionDescriptor;
            var action = descriptor.ActionName;
            var controller = descriptor.ControllerName;

            if (context.Result != null)
            {
                if (context.Result.GetType().Name != "BadRequestObjectResult" && context.Result.GetType().Name != "BadRequestResult")
                {
                    var record = ((Microsoft.AspNetCore.Mvc.OkObjectResult)context.Result).Value;
                    if (record != null)
                    {
                        System.Reflection.PropertyInfo transNo = record.GetType().GetProperty("TransactionNo");
                        System.Reflection.PropertyInfo recId = record.GetType().GetProperty("Id");
                        System.Reflection.PropertyInfo recPO = record.GetType().GetProperty("PONumber");


                        if (transNo != null)
                        {
                            if ((transNo.GetValue(record, null)) != null)
                            {
                                id = (transNo.GetValue(record, null))?.ToString();
                                type = " Transaction No. ";
                            }
                            else
                            {
                                id = (recId.GetValue(record, null)).ToString();
                                type = " ID ";
                            }
                        }
                        else
                        {
                            if (record.GetType().Name == "String")
                            {
                                id = record.ToString();
                                type = " Transaction No. ";
                            }
                            else if (recPO != null)
                            {
                                if (recPO.CustomAttributes.Count() > 0)
                                {
                                    id = (recPO.GetValue(record, null)).ToString();
                                    type = " PO No. ";
                                }
                                 
                            }

                            else
                            {
                                id = (recId.GetValue(record, null)).ToString();
                                type = " ID ";
                            }

                        }



                        //if (action == "AddPurchaseReturnDelivery")
                        //{
                        //    System.Reflection.PropertyInfo drNumber = record.GetType().GetProperty("DRNumber");
                        //    id = (drNumber.GetValue(record, null))?.ToString();
                        //    type = " DR No. ";
                        //}
                        //if (action == "AddClientReturn")
                        //{
                        //    System.Reflection.PropertyInfo ReturnDRNumber = record.GetType().GetProperty("ReturnDRNumber");
                        //    id = (ReturnDRNumber.GetValue(record, null))?.ToString();
                        //    type = " Return DR No. ";
                        //}
                        //if (action == "SaveReceiveItem")
                        //{
                        //    System.Reflection.PropertyInfo ReturnDRNumber = record.GetType().GetProperty("ReturnDRNumber");
                        //    id = (recPO.GetValue(record, null)).ToString();
                        //    type = " PO No. ";    
                        //}
                        


                    }

                    var user = this.currentUser.Claims.ToList()[1].Value;

                    var trailDetail = new UserTrail();

                    trailDetail.Action = action;
                    trailDetail.DateCreated = DateTime.Now;
                    trailDetail.Detail =  " Record affected : "  + type + id;
                    trailDetail.Transaction = action + " " + controller;
                    trailDetail.UserId = Convert.ToInt32(user);

                    this._trailService.InsertTrail(trailDetail);



                }
                else
                {
                    return;
                }
                
            }
            
            
           
           
            

          

           






        }

    }
}
