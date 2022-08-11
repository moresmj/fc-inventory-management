using System;
using System.Collections.Generic;
using AutoMapper;
using FC.Api.DTOs;
using FC.Api.DTOs.Size;
using FC.Api.Helpers;
using FC.Api.Services.NotificationHub;
using FC.Api.Services.Sizes;
using FC.Core.Domain.Sizes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FC.Api.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Sizes")]
    public class SizesController : BaseController
    {

        private ISizeService _service;
        private IMapper _mapper;
        private IHubContext<NotifyHub, ITypedHubClient> _hubContext;

        public SizesController(ISizeService service,
            IMapper mapper,
            IHubContext<NotifyHub, ITypedHubClient> hubContext)
        {
            _service = service;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _service.GetAllSizes();

            var obj = _mapper.Map<IList<SizeDTO>>(list);
            return Ok(obj);
        }

        [HttpPost]
        [Route("message")]
        public string Post([FromBody]Message msg)
        {

            string retMessage = string.Empty;
            try
            {
                _hubContext.Clients.Group("Store").BroadCastMessage("success", "test group gago");
                //_hubContext.Clients.All.BroadCastMessage(msg.Type, msg.Payload);
                retMessage = "Success";
            }
            catch(Exception e)
            {
                retMessage = e.ToString();

            }

            return retMessage;

        }


        [HttpPost]
        [Route("message2")]
        public string Post2([FromBody]Message msg)
        {

            string retMessage = string.Empty;
            try
            {
                _hubContext.Clients.All.BroadCastNotificationMain(msg.Type, msg.Payload);
                retMessage = "Success";
            }
            catch (Exception e)
            {
                retMessage = e.ToString();

            }

            return retMessage;

        }

        [ServiceFilter(typeof(UserTrailActionFilter))]
        [HttpPost]
        public IActionResult Add([FromBody]SizeDTO dto)
        {
            var obj = _mapper.Map<Size>(dto);
            _service.InsertSize(obj);
            return Ok(obj);
        }

    }
}