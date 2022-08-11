using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using FC.Api.DTOs.User;
using FC.Api.DTOs.Usertrail;
using FC.Api.Helpers;
using FC.Api.Services.Users;
using FC.Api.Services.UserTrails;
using FC.Api.Services.Warehouses;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Users;
using FC.Core.Domain.UserTrail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FC.Api.Controllers
{
    [Route("api/Users")]
    public class UsersController : BaseController
    {
        private IWHStockService _warehouseStockService;
        private IUserService _service;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private IUserTrailService _trailService;

        public UsersController(IWHStockService whStockService, IUserService service, IMapper mapper, IOptions<AppSettings> appSettings, IUserTrailService trailService)
        {
            _warehouseStockService = whStockService;
            _service = service;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _trailService = trailService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateDTO dto)
        {
            var user = _service.Authenticate(dto.UserName, dto.Password);
            
            if (user == null)
            {
                return Unauthorized();
            }
            this._trailService.InsertUserLoginTrail(_service, user);

            // Application of Store Dealer
            var storesHandled = _service.GetStoresHandled(user.Id);
            if (user.UserType == UserTypeEnum.Dealer && storesHandled.Count > 0)
            {
                user.StoreId = (int?)storesHandled[0]["id"];
                user.Store = (Store)storesHandled[0]["store"];
            }
            // --------

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var claims = new[] 
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                //Expires = DateTime.UtcNow.AddHours(_appSettings.TokenEpiration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);


            return Ok(new
            {
                user.Id,
                Username = user.UserName,
                user.FullName,
                user.UserType,
                user.Assignment,
                StoreName = (user.StoreId.HasValue) ? user.Store.Name : null,
                WarehouseName = (user.WarehouseId.HasValue) ? user.Warehouse.Code : null,
                WarehouseId = (user.WarehouseId.HasValue) ? user.WarehouseId : null,
                Token = tokenString,
                TokenExpiration = _appSettings.TokenEpiration,
                StoresHandled = storesHandled
            });
        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPost]
        public IActionResult Add([FromBody]UserDTO dto)
        {
            var userType = this.GetCurrentUserUserType(_warehouseStockService.DataContext());
            if (userType != UserTypeEnum.Administrator)
            {
                return BadRequest();
            }

            var obj = _mapper.Map<User>(dto);
            _service.InsertUser(obj, dto.Password);
            return Ok(obj);
        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPost]
        [Route("dealer")]
        public IActionResult AddDealer([FromBody]UserDealerDTO dto)
        {
            var userType = this.GetCurrentUserUserType(_warehouseStockService.DataContext());
            if (userType != UserTypeEnum.Administrator)
            {
                return BadRequest();
            }

            var obj = _mapper.Map<User>(dto);
            _service.InsertUserDealer(obj, dto.Password, dto.StoresHandled);
            return Ok(obj);
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _service.GetAllUsers();

            var dto = _mapper.Map<IList<UserDTO>>(list);
            return Ok(dto);
        }

        [HttpGet]
        [Route("dealer")]
        public IActionResult GetAllDealers()
        {
            var list = _service.GetAllUserDealer();

            var dto = _mapper.Map<IList<UserDealerDTO>>(list);
            return Ok(dto);
        }

        [HttpGet]
        [Route("forwarehouse")]
        public IActionResult GetUsersForWarehouse()
        {
            var warehouseId = this.GetCurrentUserWarehouseId(_service.DataContext());

            var list = _service.GetAllUsersByWarehouseId(warehouseId);

            var dto = _mapper.Map<IList<UserDTO>>(list);
            return Ok(dto);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int? id)
        {
            var obj = _service.GetUserById(id);
            if (obj == null)
            {
                return BadRequest();
            }

            var dto = _mapper.Map<UserDTO>(obj);
            return Ok(dto);
        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]UserDTO dto)
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

            var obj = _mapper.Map<User>(dto);
            if (obj == null)
            {
                return BadRequest();
            }

            _service.UpdateUser(obj, dto.Password);
            return Ok(obj);
        }



        [HttpPut]
        [Route("updatestatus/{id}")]
        public IActionResult UpdateStatus(int id)
        {

            var obj = _service.GetUserById(id);
            if (obj == null)

            {
                return BadRequest();
            }


            _service.UpdateUserStatus(id);

            return Ok();
        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPut("dealer/{id}")]
        public IActionResult UpdateDealer(int id, [FromBody]UserDealerDTO dto)
        {
            var userType = this.GetCurrentUserUserType(_warehouseStockService.DataContext());
            if (userType != UserTypeEnum.Administrator)

            if (id != dto.Id)
            {
                return BadRequest();
            }

            var obj = _mapper.Map<User>(dto);
            if (obj == null)
            {
                return BadRequest();
            }

            _service.UpdateUserDealer(obj, dto.StoresHandled, dto.Password);
            return Ok(obj);

        }


        [HttpGet]
        [Route("usertrail")]
        public IActionResult GetAllUserTrail(SearchUserTrail search)
        {
            var list = _trailService.GetAllUserTrail(search,_appSettings);


            return Ok(list);
        }
    }
}