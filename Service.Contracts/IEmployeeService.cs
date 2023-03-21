using Entities.Models;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Create;
using Shared.DataTransferObjects.Response;
using Shared.DataTransferObjects.Update;

namespace Service.Contracts;

public interface IEmployeeService
{
    IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges);
    EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges);
    EmployeeDto CreateEmployeeForCompany(Guid companyId, 
        EmployeeForCreationDto employeeForCreation, bool trackChanges);
    void DeleteEmployeeForCompany(Guid companyId, Guid id, bool trackChanges);
    
    // We are declaring a method that contains both id parameters – one for
    // the company and one for employee, the employeeForUpdate object sent
    // from the client, and two track changes parameters, again, one for the
    // company and one for the employee. We are doing that because we won't 
    // track changes while fetching the company entity, but we will track
    // changes while fetching the employee.
    void UpdateEmployeeForCompany(Guid companyId, Guid id, 
        EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, 
        bool empTrackChanges);
    
    (EmployeeForUpdateDto employeeToPatch, Employee employeeEntity) GetEmployeeForPatch(
        Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges);
    
    void SaveChangesForPatch(EmployeeForUpdateDto employeeToPatch, 
        Employee employeeEntity);
}