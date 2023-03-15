using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

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
    // Because there is no route attribute right above the action, the route for
    // the GetCompanies action will be api/companies which is the route 
    // placed on top of our controller.
    public IActionResult GetCompanies() 
    {
        var companies = 
            _service.CompanyService.GetAllCompanies(trackChanges: false);
        return Ok(companies);
    }
    
    // The route for this action is /api/companies/id and that’s because the 
    // /api/companies part applies from the root route (on top of the controller)
    // and the id part is applied from the action attribute [HttpGet(“{id:guid}“)].
    // You can also see that we are using a route constraint (:guid part) where we
    // explicitly state that our id parameter is of the GUID type.
    [HttpGet(template: "{id:guid}")]
    public IActionResult GetCompany(Guid id)
    {
        var company = _service.CompanyService.GetCompany(id, trackChanges: false);
        return Ok(company);
    }
}