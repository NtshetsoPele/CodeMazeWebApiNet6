using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Create;
using Shared.DataTransferObjects.Response;
using Shared.DataTransferObjects.Update;

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
    void DeleteCompany(Guid companyId, bool trackChanges);
    void UpdateCompany(Guid companyid, CompanyForUpdateDto companyForUpdate, 
        bool trackChanges);
}