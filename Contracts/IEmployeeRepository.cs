using Entities.Models;
using Shared.RequestFeatures;

namespace Contracts;

public interface IEmployeeRepository
{
    PagedList<Employee> GetEmployees(Guid companyId, 
        EmployeeParameters employeeParameters, bool trackChanges);
    Employee? GetEmployee(Guid companyId, Guid id, bool trackChanges);
    void CreateEmployeeForCompany(Guid companyId, Employee employee);
    void DeleteEmployee(Employee employee); // Deleting a child resource
}