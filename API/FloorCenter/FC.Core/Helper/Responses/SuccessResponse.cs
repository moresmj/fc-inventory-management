using System;
using System.Collections.Generic;
using System.Text;

namespace FC.Core.Helper.Responses
{
    public class SuccessResponse : BaseResponse
    {

        public SuccessResponse()
        {
            base.Success = true;
        }

    }
}
