using System.Text.Json;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects.Create;
using Shared.DataTransferObjects.Response;
using Shared.DataTransferObjects.Update;
using Shared.RequestFeatures;

namespace Presentation.Controllers;

// A single employee can’t exist without a company entity and this is exactly
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
    //public IActionResult GetEmployeesForCompany(Guid companyId)
    public IActionResult GetEmployeesForCompany(Guid companyId,
        // Using [FromQuery] to point out that we’ll be using query parameters
        // to define which page and how many employees we are requesting.
        // The EmployeeParameters class is the container for the actual
        // parameters for the Employee entity.
        [FromQuery] EmployeeParameters employeeParameters)
    {
        /*
        IEnumerable<EmployeeDto> employees = 
            _service.EmployeeService.GetEmployees(companyId, 
                employeeParameters, trackChanges: false);
        return Ok(employees);
        */

        (IEnumerable<EmployeeDto> employees, MetaData metaData) pagedResult = 
            _service.EmployeeService.GetEmployees(companyId,
            employeeParameters, trackChanges: false);
        
        // The new thing in this action is that we modify the response header
        // and add our metadata as the X-Pagination header. For this, we need
        // the System.Text.Json namespace.
        Response.Headers.Add("X-Pagination", 
            JsonSerializer.Serialize(pagedResult.metaData));
        
        return Ok(pagedResult.employees);
    }

    [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
    public IActionResult GetEmployeeForCompany(Guid companyId, Guid id)
    {
        var employee = 
            _service.EmployeeService.GetEmployee(companyId, id, trackChanges: false);
        return Ok(employee);
    }
    
    [HttpPost]
    public IActionResult CreateEmployeeForCompany(Guid companyId, 
        [FromBody] EmployeeForCreationDto? employee)
    {
        if (employee is null)
        {
            return BadRequest("EmployeeForCreationDto object is null");
        }

        // Extra error messages can be included targeting specific properties.
        // ModelState.AddModelError(key: "Age", errorMessage: "Extra message things");
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        
        var employeeToReturn = 
            _service.EmployeeService.CreateEmployeeForCompany(companyId, employee, 
                trackChanges: false);
        
        return CreatedAtRoute("GetEmployeeForCompany", 
            new { companyId, id = employeeToReturn.Id },
            employeeToReturn);
    }
    
    [HttpDelete("{id:guid}")]
    public IActionResult DeleteEmployeeForCompany(Guid companyId, Guid id)
    {
        _service.EmployeeService.DeleteEmployeeForCompany(companyId, id, 
            trackChanges: false);
        return NoContent();
    }
    
    // NOTE: We’ve changed only the Age property, but we have sent all
    // the other properties with unchanged values as well. Therefore,
    // Age is only updated in the database. But if we send the object
    // with just the Age property, other properties will be set to their
    // default values and the whole object will be updated — not just
    // the Age column. That’s because PUT is a request for a full
    // update. This is very important to know.
    [HttpPut("{id:guid}")]
    public IActionResult UpdateEmployeeForCompany(Guid companyId, Guid id, 
        [FromBody] EmployeeForUpdateDto? employee)
    {
        if (employee is null)
        {
            return BadRequest(nameof(EmployeeForUpdateDto) + " object is null");
        }

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        
        _service.EmployeeService.UpdateEmployeeForCompany(companyId, id, employee,
            compTrackChanges: false, empTrackChanges: true);
        return NoContent();
    }
    
    [HttpPatch("{id:guid}")]
    public IActionResult PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id,
        [FromBody] JsonPatchDocument<EmployeeForUpdateDto>? patchDoc)
    {
        if (patchDoc is null)
        {
            return BadRequest("patchDoc object sent from client is null.");
        }
        var result = 
            _service.EmployeeService.GetEmployeeForPatch(companyId, id, 
            compTrackChanges: false, empTrackChanges: true);
        // patchDoc.ApplyTo(result.employeeToPatch); Take not of errors - 'ModelState'
        patchDoc.ApplyTo(result.employeeToPatch, ModelState); // ApplyTo() from NewtonsoftJson
        // Validate the already patched 'employeeToPatch' instance.
        TryValidateModel(result.employeeToPatch); 
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        
        _service.EmployeeService.SaveChangesForPatch(result.employeeToPatch, 
            result.employeeEntity);
        return NoContent();
    }
}