using AutoMapper;
using FC.Api.DTOs.Item;
using FC.Api.Helpers;
using FC.Api.Services.Items;
using FC.Api.Services.UserTrails;
using FC.Api.Services.Warehouses;
using FC.Core.Domain.Items;
using FC.Core.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FC.Api.Controllers
{
    
    [Route("api/Items")]
    public class ItemsController : BaseController
    {

        private IWHStockService _warehouseStockService;
        private IItemService _service;
        private IUserTrailService _trailService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public ItemsController(IWHStockService whStockService, IItemService service, IMapper mapper, IOptions<AppSettings> appSettings,IUserTrailService trailService)
        {
            _warehouseStockService = whStockService;
            _service = service;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _trailService = trailService;
        }

        [HttpGet]
        public IActionResult GetAllItems(ItemSearchDTO dto)
        {
           

            //var list = _service.GetAllItems2(dto, _appSettings);

            var list = _service.GetAllItems(dto);
            var obj = _mapper.Map<IList<ItemDTO>>(list);


            return Ok(obj);
        }

        [Route("items2")]
        [HttpGet]
        public IActionResult GetAllItems2(ItemSearchDTO dto)
        {

            var list = _service.GetAllItems2(dto, _appSettings);


            return Ok(list);
        }
        [Route("itemfordropdown")]
        [HttpGet]
        public IActionResult GetAllItemsForDropDown()
        {
            var list = _service.GetAllItemsForDropDown();

            var obj = _mapper.Map<IList<ItemDTO>>(list);
            return Ok(obj);
        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPost]
        public IActionResult Add([FromBody]ItemWithImageDTO dto)
        {
            var user = this.GetUser(_warehouseStockService.DataContext());

            if (user.Assignment != AssignmentEnum.Warehouse)
            {
                var userType = this.GetCurrentUserUserType(_warehouseStockService.DataContext());
                if (userType != UserTypeEnum.Administrator)
                {
                    return BadRequest();
                }
            }

            if (!string.IsNullOrEmpty(dto.ImageName))
            {
                FileInfo fi = new FileInfo(Path.Combine(_appSettings.Item_temp_image, Path.GetFileName(dto.ImageName)));

                // existing file will be overwritten
                var imageFile = dto.ImageName.Split('.');
                var newFileName = "item-" + dto.SerialNumber.ToString() + "-" + dto.Code + "." + imageFile[1];
                dto.ImageName = newFileName;
                fi.CopyTo(Path.Combine(_appSettings.Item_image, newFileName), true);
            }

            //var user = this.GetUser(_warehouseStockService.DataContext());

            

            var obj = _mapper.Map<Item>(dto);
            _service.InsertItem(obj);


            //_trailService.InsertUserTrail(this.Request.Method, user.Id, "Add New Item", "Item Added with serial " + obj.SerialNumber);
            return Ok(obj);
        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]ItemWithImageDTO dto)
        {
            var userType = this.GetCurrentUserUserType(_warehouseStockService.DataContext());
            if (userType != UserTypeEnum.Administrator)
            {
                return BadRequest();
            }

            if (id != dto.Id)
            {
                return BadRequest();
            }

            var obj = _mapper.Map<Item>(dto);
            if (obj == null)
            {
                return BadRequest();
            }

            if (!string.IsNullOrEmpty(dto.ImageName))
            {
                if (!dto.ImageName.Contains("item-")) // format of saved image of item - [ item + serial + code ]
                {
                    FileInfo fi = new FileInfo(Path.Combine(_appSettings.Item_temp_image, Path.GetFileName(dto.ImageName)));

                    // existing file will be overwritten
                    var imageFile = dto.ImageName.Split('.');
                    var newFileName = "item-" + dto.SerialNumber.ToString() + "-" + dto.Code + "." + imageFile[1];
                    obj.ImageName = newFileName;
                    fi.CopyTo(Path.Combine(_appSettings.Item_image, newFileName), true);
                }
            }


            _service.UpdateItem(obj);
            return Ok(obj);
        }

        [HttpPost]
        [Route("upload")]
        public async Task Upload(IFormFile file)
        {
            if (file == null) throw new Exception("File is null");
            if (file.Length == 0) throw new Exception("File is empty");
            

            if (file.Length <= 7000000)
            {
                var filePath = Path.Combine(_appSettings.Item_temp_image, file.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
        }

    }
}