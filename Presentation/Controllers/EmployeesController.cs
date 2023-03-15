using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace Presentation.Controllers;

// a single employee can’t exist without a company entity and this is exactly
// what we are exposing through this URI.
// To get an employee or employees from the database, we have to specify the
// companyId parameter, and that is something all actions will have in common.
// For that reason, we have specified this route as our root route.
[Route("api/companies/{companyId:guid}/employees")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IServiceManager _service;
    
    public EmployeesController(IServiceManager service) => _service = service;
    
    // The companyId parameter in our action and this parameter will be mapped 
    // from the main route. For that reason, we didn’t place it in the [HttpGet] 
    // attribute as we did with the GetCompany action.
    [HttpGet]
    public IActionResult GetEmployeesForCompany(Guid companyId)
    {
        var employees = 
            _service.EmployeeService.GetEmployees(companyId, trackChanges: false);
        return Ok(employees);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetEmployeeForCompany(Guid companyId, Guid id)
    {
        var employee = 
            _service.EmployeeService.GetEmployee(companyId, id, trackChanges: false);
        return Ok(employee);
    }
}