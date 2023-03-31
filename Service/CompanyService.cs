using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Create;
using Shared.DataTransferObjects.Response;
using Shared.DataTransferObjects.Update;

namespace Service;

internal sealed class CompanyService : ICompanyService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public CompanyService(IRepositoryManager repository, 
        ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanges)
    {
        IEnumerable<Company> companies = 
            await _repository.Company.GetAllCompaniesAsync(trackChanges);
        return _mapper.Map<IEnumerable<CompanyDto>>(companies);
    }
    
    public async Task<CompanyDto> GetCompanyAsync(Guid id, bool trackChanges)
    {
        Company company = await _repository.Company.GetCompanyAsync(id, trackChanges);
        if (company is null)
        {
            throw new CompanyNotFoundException(id);
        }
        var companyDto = _mapper.Map<CompanyDto>(company);
        return companyDto;
    }
    
    public async Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto companyDto)
    {
        var companyEntity = _mapper.Map<Company>(companyDto);
        _repository.Company.CreateCompany(companyEntity);
        await _repository.SaveAsync();
        var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);
        return companyToReturn;
    }
    
    // Here, we check if ids parameter is null and if it is we stop the execution flow
    // and return a bad request response to the client. If it’s not null, we fetch all
    // the companies for each id in the ids collection. If the count of ids and
    // companies mismatch, we return another bad request response to the client.
    // Finally, we are executing the mapping action and returning the result to the
    // caller.
    public async Task<IEnumerable<CompanyDto>> GetByIdsAsync(
        IEnumerable<Guid> ids, bool trackChanges)
    {
        if (ids is null)
        {
            throw new IdParametersBadRequestException();
        }
        
        IEnumerable<Company> companyEntities = 
            await _repository.Company.GetByIdsAsync(ids, trackChanges);
        
        if (ids.Count() != companyEntities.Count())
        {
            throw new CollectionByIdsBadRequestException();
        }
        
        var companiesToReturn = 
            _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
        
        return companiesToReturn;
    }
    
    // Check if the collection is null and if it is, return a bad request. If it
    // isn’t, then we map that collection and save all the collection elements to
    // the database. Finally, we map the company collection back, take all the
    // ids as a comma-separated string, and return the Tuple with these two fields
    // as a result to the caller.
    public async Task<(IEnumerable<CompanyDto> companies, string ids)> 
        CreateCompanyCollectionAsync(IEnumerable<CompanyForCreationDto> companyCollection)
    {
        if (companyCollection is null)
        {
            throw new CompanyCollectionBadRequest();
        }
        
        var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
        foreach (Company company in companyEntities)
        {
            _repository.Company.CreateCompany(company);
        }
        
        await _repository.SaveAsync();
        
        var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
        var ids = string.Join(",", companyCollectionToReturn.Select((CompanyDto c) => c.Id));
        
        return (companies: companyCollectionToReturn, ids: ids);
    }
    
    public async Task DeleteCompanyAsync(Guid companyId, bool trackChanges)
    {
        Company company = 
            await _repository.Company.GetCompanyAsync(companyId, trackChanges);
        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }
        
        _repository.Company.DeleteCompany(company);
        await _repository.SaveAsync();
    }
    
    public async Task UpdateCompanyAsync(
        Guid companyId, CompanyForUpdateDto companyForUpdate, bool trackChanges)
    {
        Company companyEntity = 
            await _repository.Company.GetCompanyAsync(companyId, trackChanges);
        if (companyEntity is null)
        {
            throw new CompanyNotFoundException(companyId);
        }
        
        _mapper.Map(companyForUpdate, companyEntity);
        await _repository.SaveAsync();
    }
}