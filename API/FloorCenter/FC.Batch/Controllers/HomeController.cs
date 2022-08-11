using FC.Batch.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Batch.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(ViewError viewError)
        {
            this.ViewError = viewError;
        }

        private ViewError ViewError { get; }

        public IActionResult Index()
        {
            var script = "<script>setTimeout(function () {location.reload();}, 60000);</script>";

            if (this.ViewError != null && this.ViewError.HasError())
            {
                return Content(script + "<a href=\"/monitor\" target=\"_blank\">Found error while running the batch.</a>", "text/html");
            }

            
            return Content(script + "Batch is running....", "text/html");
        }
    }
}
