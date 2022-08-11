using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Size;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InventorySystemAPI.Controllers
{
    [Route("api/[controller]")]
    public class SizesController : BaseController
    {

        public SizesController(FloorCenterContext context)
            : base(context)
        {

        }

        [HttpGet]
        public IEnumerable<Size> GetAll()
        {
            return _context.Sizes.ToList();
        }

        [HttpPost]
        public IActionResult Add([FromBody] Size size)
        {
            size.DateCreated = DateTime.Now;

            _context.Sizes.Add(size);
            _context.SaveChanges();

            return new NoContentResult();
        }

    }
}
