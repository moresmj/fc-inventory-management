using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventorySystemAPI.Classes;
using InventorySystemAPI.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystemAPI.Controllers
{
    public class BaseController : Controller
    {

        protected Common _common;

        protected FloorCenterContext _context;

        protected BaseController(FloorCenterContext context)
        {
            this._common = new Common();
            this._context = context;
        }

    }

}