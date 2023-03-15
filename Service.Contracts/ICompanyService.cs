using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface ICompanyService
{
    // Let’s be clear right away before we proceed. Getting
    // all the entities from the database is a bad idea.
    // We’re going to start with the simplest method and
    // change it later on.
    IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges);
    CompanyDto GetCompany(Guid companyId, bool trackChanges);
}