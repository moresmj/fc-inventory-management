using System;
using System.Collections.Generic;
using System.Text;

namespace FC.Core.Helper.Responses
{
    public class ErrorResponse : BaseResponse
    {

        public ErrorResponse()
        {

            this.ErrorMessages = new List<object>();
        }


        public List<object> ErrorMessages { get; set; }
    }
}
