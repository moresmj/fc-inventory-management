using FC.Core.Domain.Companies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.Companies
{
    public interface ICompanyService
    {

        /// <summary>
        /// Get all companies
        /// </summary>
        /// <returns>Company List</returns>
        IEnumerable<Company> GetAllCompanies();

        /// <summary>
        ///  Add new company
        /// </summary>
        /// <param name="company"></param>
        void AddCompany(Company company);

        /// <summary>
        /// Update Company
        /// </summary>
        /// <param name="company"></param>
        void UpdateCompany(Company company);

    }
}
