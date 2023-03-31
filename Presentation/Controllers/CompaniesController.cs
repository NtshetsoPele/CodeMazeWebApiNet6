using Microsoft.AspNetCore.Mvc;
using Presentation.ModelBinders;
using Service.Contracts;
using Shared.DataTransferObjects.Create;
using Shared.DataTransferObjects.Response;
using Shared.DataTransferObjects.Update;

namespace Presentation.Controllers;
// Controllers should only be responsible for
// handling requests, model validation, and
// returning responses to the frontend or some
// HTTP client.

// Keeping business logic away from controllers
// is a good way to keep them lightweight, and
// our code more readable and maintainable.

// The purpose of the presentation layer is to
// provide the entry point to our system so that
// consumers can interact with the data.
// Examples (REST API, gRPC, etc).

// By convention, controllers are defined in the
// Controllers folder inside the main project.

// ASP.NET Core uses Dependency Injection everywhere,
// we need to have a reference to all of the projects
// in the solution from the main project. This allows
// us to configure our services inside the Program
// class.
// What’s preventing our controllers from injecting
// anything they want inside the constructor?
[ApiController]
// The [ApiController] attribute is applied to a controller
// class to enable the following opinionated, API-specific
// behaviors:
//    Attribute routing requirement
//    Automatic HTTP 400 responses
//    Binding source parameter inference
//    Multipart/form-data request inference
//    Problem details for error status codes
// Web API routing routes incoming HTTP requests to
// the particular action method inside the Web API
// controller.
// The MVC framework parses that request and tries
// to match it to an action in the controller.

// We can configure this type of routing in the Program
// class:
// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");
// Our Web API project doesn’t configure routes this way,
// but if you create an MVC project this will be the
// default route configuration. Of course, if you are
// using this type of route configuration, you have to
// use the app.UseRouting method to add the routing
// middleware in the application’s pipeline.
// If you inspect the Program class in our main project,
// you won’t find the UseRouting method because the
// routes are configured with the app.MapControllers
// method, which adds endpoints for controller actions
// without specifying any routes.

// Attribute routing uses the attributes to map the
// routes directly to the action methods inside the
// controller.
// Usually, we place the base route above the controller
// class.
// For the specific action methods, we create their
// routes right above them.
// While working with the Web API project, the ASP.NET
// Core team suggests that we shouldn’t use
// Convention-based Routing, but Attribute routing
// instead.
// Change the base route from [Route("api/[controller]")]
// to [Route("api/companies")]. Even though the first route
// will work just fine, with the second example we are more
// specific to show that this routing should point to the
// CompaniesController class.
// [Route("api/[controller]")]
[Route("api/companies")]
// Every web API controller class inherits from the
// ControllerBase abstract class, which provides all
// necessary behavior for the derived class.
public class CompaniesController : ControllerBase
{
    private readonly IServiceManager _service;
    
    public CompaniesController(IServiceManager service) => 
        _service = service;
    
    // Maps this action to a GET request.
    [HttpGet]
    // The IActionResult interface supports using a variety of methods, which
    // return not only the result but also the status codes. In this situation,
    // the OK method returns all the companies and also the status code 200 —
    // which stands for OK. If an exception occurs, we are going to return the
    // internal server error with the status code 500.
    // As there's no route attribute right above the action, the route for
    // the GetCompanies action will be api/companies which is the route 
    // placed on top of our controller.
    public async Task<IActionResult> GetCompanies() 
    {
        IEnumerable<CompanyDto> companies = 
            await _service.CompanyService.GetAllCompaniesAsync(trackChanges: false);
        
        return Ok(companies);
    }
    
    // The route for this action is /api/companies/id and that’s because the 
    // /api/companies part applies from the root route (on top of the controller)
    // and the id part is applied from the action attribute [HttpGet(“{id:guid}“)].
    // You can also see that we are using a route constraint (':guid' part) where
    // we explicitly state that our id parameter is of the GUID type.
    
    // Sets the name for the action.
    // This name will come in handy in the action method for creating a new company.
    [HttpGet(template: "{id:guid}", Name = "CompanyById")] 
    public async Task<IActionResult> GetCompany(Guid id)
    {
        CompanyDto company = 
            await _service.CompanyService.GetCompanyAsync(id, trackChanges: false);
        
        return Ok(company);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
    {
        // Because the company parameter comes from the client, it could happen that it
        // can’t be deserialized. As a result, we have to validate it against the reference
        // type’s default value, which is null.
        if (company is null)
        {
            return BadRequest(nameof(CompanyForCreationDto) + " object is null");
        }
        
        CompanyDto createdCompany = 
            await _service.CompanyService.CreateCompanyAsync(company);
        
        // CreatedAtRoute will return a status code 201, which stands for Created. Also,
        // it will populate the body of the response with the new company object as well
        // as the Location attribute within the response header with the address to retrieve
        // that company. We need to provide the name of the action, where we can retrieve
        // the created entity.
        return CreatedAtRoute(
            "CompanyById", new { id = createdCompany.Id }, createdCompany);
    }
    
    // Our 'ArrayModelBinder' will be triggered before an action executes. It will
    // convert the sent string parameter to the IEnumerable<Guid> type, and then
    // the action will be executed
    [HttpGet(template: "collection/({ids})", Name = "CompanyCollection")]
    public async Task<IActionResult> GetCompanyCollection(
        [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
    {
        IEnumerable<CompanyDto> companies = 
            await _service.CompanyService.GetByIdsAsync(ids, trackChanges: false);
        
        return Ok(companies);
    }
    
    // Why are we sending a comma-separated string when we expect a collection
    // of ids in the GetCompanyCollection action?
    // Well, we can’t just pass a list of ids in the 'CreatedAtRoute' method because
    // there is no support for the Header Location creation with the list.
    [HttpPost("collection")]
    public async Task<IActionResult> CreateCompanyCollection(
        [FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
    {
        (IEnumerable<CompanyDto> companies, string ids) result = 
            await _service.CompanyService.CreateCompanyCollectionAsync(companyCollection);
        
        return CreatedAtRoute(
            "CompanyCollection", new { result.ids }, result.companies);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        await _service.CompanyService.DeleteCompanyAsync(id, trackChanges: false);
        
        return NoContent();
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCompany(
        Guid id, [FromBody] CompanyForUpdateDto company)
    {
        if (company is null)
        {
            return BadRequest($"'{nameof(CompanyForUpdateDto)}' object is null");
        }
        
        await _service.CompanyService.UpdateCompanyAsync(id, company, trackChanges: true);
        
        return NoContent();
    }
}