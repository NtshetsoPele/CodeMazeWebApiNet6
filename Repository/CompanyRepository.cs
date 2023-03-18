using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class CompanyRepository: RepositoryBase<Company>, ICompanyRepository
{
    public CompanyRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public IEnumerable<Company> GetAllCompanies(bool trackChanges)
    {
        var allCompaniesQuery = 
            FindAll(trackChanges).OrderBy((Company c) => c.Name);
        Console.WriteLine($"allCompaniesQuery: '{allCompaniesQuery.ToQueryString()}'.");
        return allCompaniesQuery.ToList();
    }

    public Company? GetCompany(Guid companyId, bool trackChanges)
    {
        return FindByCondition(c 
                => c.Id.Equals(companyId), trackChanges).SingleOrDefault();
    }
    
    public void CreateCompany(Company company) => Create(company);

    public IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges)
    {
        return FindByCondition(
                (Company c) => ids.Contains(c.Id), trackChanges)
            .ToList();
    }
}