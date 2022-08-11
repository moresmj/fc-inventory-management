using AutoMapper;
using FC.Api.DTOs.Item;
using FC.Api.Services.Items;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FC.Api.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Categories")]
    public class CategoriesController : Controller
    {

        private ICategoryParentService _parentService;
        private ICategoryChildService _childService;
        private ICategoryGrandChildService _grandChildService;
        private IMapper _mapper;

        public CategoriesController(ICategoryParentService parentService, ICategoryChildService childService, ICategoryGrandChildService grandChildService, IMapper mapper)
        {
            _parentService = parentService;
            _childService = childService;
            _grandChildService = grandChildService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("all")]
        public IActionResult GetAllParentsChildrenGrandChildrenCategories()
        {
            var list = _parentService.GetAllParentsChildrenGrandChildrenCategories();

            var obj = _mapper.Map<IList<CategoryParentDTO>>(list);
            return Ok(obj);
        }

        [HttpGet]
        [Route("all/{id}")]
        public IActionResult GetParentAllChildrenGrandChildrenCategories(int? id)
        {
            var list = _parentService.GetParentAllChildrenGrandChildrenCategories(id);

            var obj = _mapper.Map<IList<CategoryParentDTO>>(list);
            return Ok(obj);
        }
        
        [HttpGet]
        [Route("parentchild")]
        public IActionResult GetParentAllChildrenGrandChildrenCategories()
        {
            var list = _parentService.GetAllParentsAndChildrenCategories();

            var obj = _mapper.Map<IList<CategoryParentDTO>>(list);
            return Ok(obj);
        }

        [HttpGet]
        [Route("parentchild/{id}")]
        public IActionResult GetParentAllChildrenCategories(int? id)
        {
            var list = _parentService.GetParentAllChildrenCategories(id);

            var obj = _mapper.Map<IList<CategoryParentDTO>>(list);
            return Ok(obj);
        }

        [HttpGet]
        [Route("childgrandchild")]
        public IActionResult GetAllChildGrandChildCategories()
        {
            var list = _childService.GetAllChildGrandChildCategories();

            var obj = _mapper.Map<IList<CategoryChildDTO>>(list);
            return Ok(obj);
        }

        [HttpGet]
        [Route("childgrandchild/{id}")]
        public IActionResult GetChildAllGrandChildCategories(int? id)
        {
            var list = _childService.GetChildAllGrandChildCategories(id);

            var obj = _mapper.Map<IList<CategoryChildDTO>>(list);
            return Ok(obj);
        }

    }
}