using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Request;
using Shared.DataTransferObjects.Response;

namespace Service.Contracts;

public interface ICompanyService
{
    // Let’s be clear right away before we proceed. Getting
    // all the entities from the database is a bad idea.
    // We’re going to start with the simplest method and
    // change it later on.
    IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges);
    CompanyDto GetCompany(Guid companyId, bool trackChanges);
    CompanyDto CreateCompany(CompanyForCreationDto company);
    IEnumerable<CompanyDto> GetByIds(IEnumerable<Guid> ids, bool trackChanges);
    (IEnumerable<CompanyDto> companies, string ids) CreateCompanyCollection
        (IEnumerable<CompanyForCreationDto> companyCollection);
}