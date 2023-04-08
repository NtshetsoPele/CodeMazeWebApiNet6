using Contracts;
using Entities.Models;
using Shared.RequestFeatures;

namespace Repository;
    
public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public PagedList<Employee> GetEmployees(Guid companyId, 
        EmployeeParameters employeeParameters, bool trackChanges)
    {
        /*
        IEnumerable<Employee> employees = FindByCondition((Employee e) 
                => e.CompanyId.Equals(companyId), trackChanges)
            .OrderBy((Employee e) => e.Name)
            .ToList(); // All of them? Yes. See below improvement.
        
        return PagedList<Employee>
            .ToPagedList(employees, employeeParameters.PageNumber, 
                employeeParameters.PageSize);
        */
        
        // Even though we have an additional call to the database with the 
        // CountAsync method, this solution was tested upon millions of rows and 
        // was much faster than the previous one.
        IEnumerable<Employee> employees = FindByCondition((Employee e) 
                => e.CompanyId.Equals(companyId), trackChanges)
            .OrderBy((Employee e) => e.Name)
            .Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
            .Take(employeeParameters.PageSize)
            .ToList();
        
        int count = FindByCondition((Employee e) 
                => e.CompanyId.Equals(companyId), trackChanges).Count();
        
        return new PagedList<Employee>(employees, count, 
            employeeParameters.PageNumber, employeeParameters.PageSize);
    }

    public Employee? GetEmployee(Guid companyId, Guid id, bool trackChanges)
    {
        return FindByCondition(e => 
                    e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges)
            .SingleOrDefault();
    }
    
    public void CreateEmployeeForCompany(Guid companyId, Employee employee)
    {
        employee.CompanyId = companyId;
        Create(employee);
    }
    
    public void DeleteEmployee(Employee employee) => Delete(employee);
}