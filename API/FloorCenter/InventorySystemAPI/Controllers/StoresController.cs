using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystemAPI.Classes;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InventorySystemAPI.Controllers
{
    [Route("api/[controller]")]
    public class StoresController : BaseController
    {

        public StoresController(FloorCenterContext context) 
            : base(context)
        {

        }


        [HttpGet]
        public IEnumerable<Store> GetAll()
        {

            var records = (from x in _context.Stores
                          .Include("Company")
                          .Include("Warehouse")
                           select x);

            return records;
        }


        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var obj = _context.Stores.FirstOrDefault(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            return new ObjectResult(obj);
        }

        [HttpGet]
        [Route("search")]
        public IEnumerable<Store> search(StoreSearch search)
        {
            var criteria = _common.getModelPropertyWithValueAsDictionary(search);
            var records = (from x in _context.Stores.Where(x => _common.filterModelLike(x, criteria))
              .Include("Company")
              .Include("Warehouse")
                           select x);

            return records;
        }


        [HttpPost]
        public IActionResult Add([FromBody] Store store)
        {
            store.DateCreated = DateTime.Now;

            _context.Stores.Add(store);
            _context.SaveChanges();

            return new NoContentResult();
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Store store)
        {
            if (store.Id != id)
            {
                return BadRequest();
            }

            var obj = _context.Stores.FirstOrDefault(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            obj.Name = store.Name;
            obj.Address = store.Address;
            obj.ContactNumber = store.ContactNumber;
            obj.CompanyId = store.CompanyId;
            obj.WarehouseId = store.WarehouseId;
            obj.DateUpdated = DateTime.Now;

            _context.Stores.Update(obj);
            _context.SaveChanges();

            return new NoContentResult();
        }
    }
}
