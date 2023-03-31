using Entities.Models;

namespace Contracts;

public interface ICompanyRepository 
{
    // The Create and Delete method signatures are left synchronous. That’s 
    // because, in these methods, we are not making any changes in the
    // database. All we're doing is changing the state of the entity to Added
    // and Deleted.
    Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges);
    Task<Company> GetCompanyAsync(Guid companyId, bool trackChanges);
    Task<IEnumerable<Company>> GetByIdsAsync(
        IEnumerable<Guid> ids, bool trackChanges);
    // IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges);
    // IEnumerable<Company> GetAllCompanies(bool trackChanges);
    // Company? GetCompany(Guid companyId, bool trackChanges);
    void CreateCompany(Company company);
    void DeleteCompany(Company company);
}