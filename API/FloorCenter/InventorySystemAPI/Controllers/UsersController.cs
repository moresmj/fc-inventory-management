using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.User;
using System;
using InventorySystemAPI.Classes;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InventorySystemAPI.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : BaseController
    {
        
        public UsersController(FloorCenterContext context) 
            : base(context)
        {
            
        }


        [HttpGet]
        public IEnumerable<User> GetAll()
        {
            return _context.Users.ToList();
        }


        [HttpGet]
        [Route("search")]
        public IEnumerable<User> Search(UserSearch user)
        {
            var criteria = _common.getModelPropertyWithValueAsDictionary(user);
            var records = (from x in _context.Users.Where(x => _common.filterModelLike(x, criteria))
                           select x);

            return records;
        }


        [HttpPost]
        public IActionResult Add([FromBody] User user)
        {
            user.DateCreated = DateTime.Now;

            if (user.Assignment != AssignmentEnum.Warehouse)
            {
                user.WarehouseId = null;
            }

            if (user.Assignment != AssignmentEnum.Store)
            {
                user.StoreId = null;
            }

            user.EncryptPassword();
            

            _context.Users.Add(user);
            _context.SaveChanges();

            return new NoContentResult();
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] User user)
        {
            if(user.Id != id)
            {
                return BadRequest();
            }

            var obj = _context.Users.FirstOrDefault(x => x.Id == id);
            if(obj == null)
            {
                return NotFound();
            }

            obj.FullName = user.FullName;
            obj.EmailAddress = user.EmailAddress;
            obj.Address = user.Address;
            obj.ContactNumber = user.ContactNumber;
            obj.Password = user.Password;
            obj.Assignment = user.Assignment;

            if (user.Assignment == AssignmentEnum.Warehouse)
            {
                obj.WarehouseId = user.WarehouseId;
            }
            else
            {
                obj.WarehouseId = null;
            }

            if (user.Assignment == AssignmentEnum.Store)
            {
                obj.StoreId = user.StoreId;
            }
            else
            {
                obj.StoreId = null;
            }

            obj.UserType = user.UserType;
            obj.DateUpdated = DateTime.Now;

            obj.EncryptPassword();

            _context.Users.Update(obj);
            _context.SaveChanges();

            return new NoContentResult();
        }
        
    }
}
