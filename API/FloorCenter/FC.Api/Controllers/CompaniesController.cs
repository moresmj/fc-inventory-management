using System.Collections.Generic;
using AutoMapper;
using FC.Api.DTOs.Company;
using FC.Api.Services.Companies;
using FC.Core.Domain.Companies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FC.Api.Controllers
{
    //[Authorize]
    [Produces("application/json")]
    [Route("api/Companies")]
    public class CompaniesController : Controller
    {
        
        private ICompanyService _service;
        private IMapper _mapper;

        public CompaniesController(ICompanyService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _service.GetAllCompanies();

            var obj = _mapper.Map<IList<CompanyDTO>>(list);
            return Ok(obj);
        }

        [HttpPost]
        public IActionResult Add([FromBody]CompanyDTO company)
        {

            var obj = _mapper.Map<Company>(company);

            _service.AddCompany(obj);
            
            return Ok(obj);
        }


        [HttpPut("{id}")]
        public IActionResult Update(int id,[FromBody]CompanyDTO company)
        {
            var obj = _mapper.Map<Company>(company);
            _service.UpdateCompany(obj);

            return Ok(obj);
        }

    }
}