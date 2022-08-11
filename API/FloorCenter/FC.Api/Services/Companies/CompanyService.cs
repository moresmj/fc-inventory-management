using System;
using System.Collections.Generic;
using System.Linq;
using FC.Api.Helpers;
using FC.Core.Domain.Companies;

namespace FC.Api.Services.Companies
{
    public class CompanyService : ICompanyService
    {

        private DataContext _context;

        public CompanyService(DataContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Get all companies
        /// </summary>
        /// <returns>Company List</returns>
        public IEnumerable<Company> GetAllCompanies()
        {
            return _context.Companies.OrderByDescending(p => p.DateCreated);
        }


        public void AddCompany(Company company)
        {
            company.DateCreated = DateTime.Now;

            _context.Companies.Add(company);
            _context.SaveChanges();
        }

        public void UpdateCompany(Company company)
        {
            var comp = _context.Companies.Where(x => x.Id == company.Id).SingleOrDefault();

            comp.DateUpdated = DateTime.Now;
            comp.Name = company.Name;
            comp.Code = company.Code;
            comp.Id = company.Id;

            _context.Companies.Update(comp);
            _context.SaveChanges();

           

        }
    }
}
