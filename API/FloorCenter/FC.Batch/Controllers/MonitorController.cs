using FC.Batch.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Batch.Controllers
{
    public class MonitorController : Controller
    {
        public MonitorController(ViewError viewError)
        {
            this.ViewError = viewError;
        }

        private ViewError ViewError { get; }

        public string Index()
        {
            return this.ViewError?.GetErrorToString() ?? "No error found";
        }

        public IActionResult Clear()
        {
            this.ViewError?.Clear();
            return Redirect("/monitor");
        }
    }
}
