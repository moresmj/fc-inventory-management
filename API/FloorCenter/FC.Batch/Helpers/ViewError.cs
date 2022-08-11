using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Batch.Helpers
{
    public class ViewError
    {
        private Exception Error { get; set; }

        public void Log(Exception e)
        {
            this.Error = e;
        }

        public string GetErrorToString()
        {
            return this.Error?.ToString();
        }

        public bool HasError()
        {
            return !string.IsNullOrEmpty(this.GetErrorToString());
        }

        public void Clear()
        {
            this.Error = null;
        }
    }
}
