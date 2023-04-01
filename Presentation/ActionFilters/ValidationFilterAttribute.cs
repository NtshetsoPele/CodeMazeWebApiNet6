using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.ActionFilters;

// Reusable for the 'POST' and 'PUT' actions for both
// the 'Company' and 'Employee' DTO objects
public class ValidationFilterAttribute : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // 'context' parameter to retrieve different values that we
        // need inside this method.
        // 'context.RouteData.Values' dictionary, we can get the
        // values produced by routes on the current routing path.
        object? action = context.RouteData.Values["action"];
        object? controller = context.RouteData.Values["controller"];
        // 'context.ActionArguments' dictionary is used to extract the
        // DTO parameter that we send to the POST and PUT actions.
        var param = context.ActionArguments
            .SingleOrDefault((KeyValuePair<string, object?> arg) => 
                arg.Value.ToString().Contains("Dto")).Value;
        
        // As client requests may have action arguments, it could happen that they
        // can’t be deserialized. As a result, we have to validate them against
        // the reference type’s default value, which is null.
        if (param is null)
        {
            context.Result = new BadRequestObjectResult(
                $"Object is null. Controller: {controller}, action: {action}");
            return;
        }

        if (!context.ModelState.IsValid)
        {
            context.Result = new UnprocessableEntityObjectResult(context.ModelState);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        throw new NotImplementedException();
    }
}