using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Request;
using Shared.DataTransferObjects.Response;

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

    public IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges)
    {
        var companies = 
            _repository.Company.GetAllCompanies(trackChanges);
        return _mapper.Map<IEnumerable<CompanyDto>>(companies);
    }
    
    public CompanyDto GetCompany(Guid id, bool trackChanges)
    {
        var company = _repository.Company.GetCompany(id, trackChanges);
        if (company is null)
            throw new CompanyNotFoundException(id);
        var companyDto = _mapper.Map<CompanyDto>(company);
        return companyDto;
    }
    
    public CompanyDto CreateCompany(CompanyForCreationDto companyDto)
    {
        var companyEntity = _mapper.Map<Company>(companyDto);
        _repository.Company.CreateCompany(companyEntity);
        _repository.Save();
        var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);
        return companyToReturn;
    }
    
    // Here, we check if ids parameter is null and if it is we stop the execution flow
    // and return a bad request response to the client. If it’s not null, we fetch all
    // the companies for each id in the ids collection. If the count of ids and
    // companies mismatch, we return another bad request response to the client.
    // Finally, we are executing the mapping action and returning the result to the
    // caller.
    public IEnumerable<CompanyDto> GetByIds(IEnumerable<Guid> ids, bool trackChanges)
    {
        if (ids is null)
        {
            throw new IdParametersBadRequestException();
        }
        var companyEntities = _repository.Company.GetByIds(ids, trackChanges);
        if (ids.Count() != companyEntities.Count())
        {
            throw new CollectionByIdsBadRequestException();
        }
        var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
        return companiesToReturn;
    }
    
    // Check if the collection is null and if it is, return a bad request. If it
    // isn’t, then we map that collection and save all the collection elements to
    // the database. Finally, we map the company collection back, take all the
    // ids as a comma-separated string, and return the Tuple with these two fields
    // as a result to the caller.
    public (IEnumerable<CompanyDto> companies, string ids) CreateCompanyCollection
        (IEnumerable<CompanyForCreationDto> companyCollection)
    {
        if (companyCollection is null)
        {
            throw new CompanyCollectionBadRequest();
        }
        var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
        foreach (var company in companyEntities)
        {
            _repository.Company.CreateCompany(company);
        }
        _repository.Save();
        var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
        var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
        return (companies: companyCollectionToReturn, ids: ids);
    }
}