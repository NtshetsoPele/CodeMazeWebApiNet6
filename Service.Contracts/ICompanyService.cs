using Shared.DataTransferObjects.Create;
using Shared.DataTransferObjects.Response;
using Shared.DataTransferObjects.Update;

namespace Service.Contracts;

public interface ICompanyService
{
    // To be clear, getting all the entities from the database
    // is a bad idea. 
    // IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges);
    // CompanyDto GetCompany(Guid companyId, bool trackChanges);
    // CompanyDto CreateCompany(CompanyForCreationDto company);
    // IEnumerable<CompanyDto> GetByIds(IEnumerable<Guid> ids, bool trackChanges);
    // (IEnumerable<CompanyDto> companies, string ids) CreateCompanyCollection
    //     (IEnumerable<CompanyForCreationDto> companyCollection);
    // void DeleteCompany(Guid companyId, bool trackChanges);
    // void UpdateCompany(Guid companyId, CompanyForUpdateDto companyForUpdate, 
    //     bool trackChanges);
    Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanges);
    Task<CompanyDto> GetCompanyAsync(Guid companyId, bool trackChanges);
    Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company);
    Task<IEnumerable<CompanyDto>> GetByIdsAsync(
        IEnumerable<Guid> ids, bool trackChanges);
    Task<(IEnumerable<CompanyDto> companies, string ids)> 
        CreateCompanyCollectionAsync(IEnumerable<CompanyForCreationDto> companyCollection);
    Task DeleteCompanyAsync(Guid companyId, bool trackChanges);
    Task UpdateCompanyAsync(
        Guid companyId, CompanyForUpdateDto companyForUpdate, bool trackChanges);
}