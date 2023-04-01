using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects.Create;
using Shared.DataTransferObjects.Response;
using Shared.DataTransferObjects.Update;

namespace Service;

public class EmployeeService : IEmployeeService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public EmployeeService(IRepositoryManager repository, 
        ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges)
    {
        CheckIfCompanyExists(companyId, trackChanges).Wait();
        
        IEnumerable<Employee> employeeEntities = 
            _repository.Employee.GetEmployees(companyId, trackChanges);
        var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(employeeEntities);
        
        return employeeDtos;
    }
    
    public EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges)
    {
        CheckIfCompanyExists(companyId, trackChanges).Wait();

        Employee employeeEntity = 
            GetEmployeeForCompanyAndCheckIfItExists(companyId, id, trackChanges);
        var employee = _mapper.Map<EmployeeDto>(employeeEntity);
        return employee;
    }
    
    public EmployeeDto CreateEmployeeForCompany(Guid companyId, 
        EmployeeForCreationDto employeeForCreation, bool trackChanges) 
    {
        // We have to check whether that company exists in the database because 
        // there is no point in creating an employee for a company that does not 
        // exist.
        CheckIfCompanyExists(companyId, trackChanges).Wait();
        var employeeEntity = _mapper.Map<Employee>(employeeForCreation);
        _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
        _repository.SaveAsync().Wait();
        var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);
        return employeeToReturn;
    }
    
    // DELETE isn’t safe because it deletes the resource, thus changing the resource
    // representation. However, if we try to send this delete request one or even
    // more times, we would get the same 404 result because the resource doesn’t
    // exist anymore. That’s what makes the DELETE request idempotent.
    public void DeleteEmployeeForCompany(Guid companyId, Guid id, bool trackChanges)
    {
        CheckIfCompanyExists(companyId, trackChanges).Wait();

        Employee? employeeForCompany =
            GetEmployeeForCompanyAndCheckIfItExists(companyId, id, trackChanges);
        _repository.Employee.DeleteEmployee(employeeForCompany);
        _repository.SaveAsync();
    }
    
    // The trackChanges parameter will be set to true for the employeeEntity.
    // That’s because we want EF Core to track changes on this entity.
    // This means that as soon as we change any property in this entity,
    // EF Core will set the state of that entity to Modified.
    public void UpdateEmployeeForCompany(Guid companyId, Guid id, 
        EmployeeForUpdateDto employeeForUpdate,
        bool compTrackChanges, bool empTrackChanges)
    {
        CheckIfCompanyExists(companyId, compTrackChanges).Wait();

        Employee employeeEntity = 
            GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);
        
        // As the entity has a modified state, it is enough to call the
        // 'Save' method without any additional update actions. As soon as we call
        // the 'Save' method, our entity is going to be updated in the database.
        
        // This update action is a connected update (an update were we use the
        // same context object to fetch the entity and to update it).
        _mapper.Map(employeeForUpdate, employeeEntity);
        _repository.SaveAsync();
    }
    
    public (EmployeeForUpdateDto employeeToPatch, Employee employeeEntity) 
        GetEmployeeForPatch
        (Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
    {
        CheckIfCompanyExists(companyId, compTrackChanges).Wait();

        Employee employeeEntity = 
            GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);
        
        // The patchDoc variable can apply only to the EmployeeForUpdateDto type.
        var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);
        return (employeeToPatch, employeeEntity);
    }
    
    public void SaveChangesForPatch(
        EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
    {
        _mapper.Map(employeeToPatch, employeeEntity);
        _repository.SaveAsync();
    }
    
    private async Task CheckIfCompanyExists(Guid companyId, bool trackChanges)
    {
        Company company = 
            await _repository.Company.GetCompanyAsync(companyId, trackChanges);
        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }
    }
    
    private Employee GetEmployeeForCompanyAndCheckIfItExists
        (Guid companyId, Guid id, bool trackChanges)
    {
        Employee? employeeDb = 
            _repository.Employee.GetEmployee(companyId, id, trackChanges);
        if (employeeDb is null)
        {
            throw new EmployeeNotFoundException(id);
        }
        return employeeDb;
    }
}