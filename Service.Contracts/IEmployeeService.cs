using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Request;
using Shared.DataTransferObjects.Response;

namespace Service.Contracts;

public interface IEmployeeService
{
    IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges);
    EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges);
    EmployeeDto CreateEmployeeForCompany(Guid companyId, 
        EmployeeForCreationDto employeeForCreation, bool trackChanges);
}