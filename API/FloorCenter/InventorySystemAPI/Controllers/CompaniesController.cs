using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Company;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystemAPI.Controllers
{
    [Route("api/[controller]")]
    public class CompaniesController : BaseController
    {

        public CompaniesController(FloorCenterContext context)
            : base(context)
        {
            
        }

        [HttpGet]
        public IEnumerable<Company> GetAll()
        {
            return _context.Companies.ToList();
        }

        [HttpPost]
        public IActionResult Add([FromBody] Company company)
        {
            company.DateCreated = DateTime.Now;

            _context.Companies.Add(company);
            _context.SaveChanges();

            return new NoContentResult();
        }

    }
}