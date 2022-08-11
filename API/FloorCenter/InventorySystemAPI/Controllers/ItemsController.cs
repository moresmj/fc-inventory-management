using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using InventorySystemAPI.Models.Item;
using InventorySystemAPI.Context;
using System;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InventorySystemAPI.Controllers
{
    [Route("api/[controller]")]
    public class ItemsController : BaseController
    {
        

        public ItemsController(FloorCenterContext context)
        : base(context)
        {

        }

        public IEnumerable<Item> Get(ItemSearch search)
        {
            var criteria = _common.getModelPropertyWithValueAsDictionary(search);
            var records = (from x in _context.Items.Where(x => _common.filterModelEqual(x, criteria))
              .Include("Size")
                           select x);

            return records;
        }     

        [HttpPost]
        public IActionResult Add([FromBody] Item item)
        {
            item.DateCreated = DateTime.Now;

            _context.Items.Add(item);
            _context.SaveChanges();

            return new NoContentResult();
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Item item)
        {
            if (item.Id != id)
            {
                return BadRequest();
            }

            var obj = _context.Items.FirstOrDefault(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            obj.Name = item.Name;
            obj.SerialNumber = item.SerialNumber;
            obj.Code = item.Code;
            obj.Description = item.Description;
            obj.SizeId = item.SizeId;
            obj.Tonality = item.Tonality;
            obj.DateUpdated = DateTime.Now;

            _context.Items.Update(obj);
            _context.SaveChanges();

            return new NoContentResult();
        }


    }
}
