using Entities.Models;

namespace Repository.Extensions;

public static class RepositoryEmployeeExtensions
{
    public static IQueryable<Employee> FilterEmployees(
        this IQueryable<Employee> employees, uint minAge, uint maxAge)
    {
        return employees.Where((Employee e) => 
            e.Age >= minAge && e.Age <= maxAge);
    }
    
    public static IQueryable<Employee> Search(
        this IQueryable<Employee> employees, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return employees;
        }
        string lowerCaseTerm = searchTerm.Trim().ToLower();
        
        return employees.Where((Employee e) => e.Name.ToLower().Contains(lowerCaseTerm));
    }
}