using FC.Api.DTOs;
using FC.Api.Services.Logger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FC.Api.Helpers
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerService _logger;

        public ExceptionMiddleware(RequestDelegate next)
        {
            this._next = next;
        }


        public async Task Invoke(HttpContext context, ILoggerService logger)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch(Exception ex)
            {

                await HandleExceptionAsync(context, ex, logger);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex,ILoggerService logger)
        {
            
            var currentRouteData = context.GetRouteData();
            var currentController = currentRouteData?.Values["controller"].ToString();
            var currentAction = currentRouteData?.Values["action"].ToString();
            var currentJwt = context.Request.Headers["Authorization"];

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            logger.WriteLogs(currentController, currentAction, ex.Message, ex.StackTrace, currentJwt);

            var errorResponse = JsonConvert.SerializeObject(new ReturnErrorModel()
            {
                Errors = "Internal Server Error."
            });

            return context.Response.WriteAsync(errorResponse);
        }
    }
}
