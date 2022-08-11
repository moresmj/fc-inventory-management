using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Warehouse;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InventorySystemAPI.Controllers
{
    [Route("api/[controller]")]
    public class WarehousesController : BaseController
    {

        public WarehousesController(FloorCenterContext context) 
            : base(context)
        {

        }


        [HttpGet]
        public IEnumerable<Warehouse> GetAll()
        {
            return _context.Warehouses.ToList();
        }

        [HttpGet]
        [Route("search")]
        public IEnumerable<Warehouse> Search(WarehouseSearch user)
        {
            var criteria = _common.getModelPropertyWithValueAsDictionary(user);
            var records = (from x in _context.Warehouses.Where(x => _common.filterModelLike(x, criteria))
                           select x);

            return records;
        }


        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var obj = _context.Warehouses.FirstOrDefault(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            return new ObjectResult(obj);
        }


        [HttpPost]
        public IActionResult Add([FromBody] Warehouse warehouse)
        {
            warehouse.DateCreated = DateTime.Now;
            _context.Warehouses.Add(warehouse);
            _context.SaveChanges();

            return new NoContentResult();
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Warehouse warehouse)
        {
            if (warehouse.Id != id)
            {
                return BadRequest();
            }

            var obj = _context.Warehouses.FirstOrDefault(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            obj.Name = warehouse.Name;
            obj.Address = warehouse.Address;
            obj.ContactNumber = warehouse.ContactNumber;
            obj.DateUpdated = DateTime.Now;
            _context.Warehouses.Update(obj);
            _context.SaveChanges();

            return new NoContentResult();
        }
    }
}
