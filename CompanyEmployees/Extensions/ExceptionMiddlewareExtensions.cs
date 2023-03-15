using Contracts;
using Entities.ErrorModel;
using Entities.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace CompanyEmployees.Extensions;

// Exception handling helps us deal with the unexpected behavior of our system. To handle
// exceptions, we use the try-catch block in our code as well as the finally keyword to clean
// up our resources afterward.
// Even though there is nothing wrong with the try-catch blocks in our Actions and methods in
// the Web API project, we can extract all the exception handling logic into a single
// centralized place. By doing that, we make our actions cleaner, more readable, and the error
// handling process more maintainable.
// In this chapter, we are going to refactor our code to use the built-in middleware for global
// error handling to demonstrate the benefits of this approach.
public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this WebApplication app, ILoggerManager logger)
    {
        // The UseExceptionHandler middleware is a built-in middleware that we can use to handle
        // exceptions.
        app.UseExceptionHandler((IApplicationBuilder appBuilder) =>
        {
            appBuilder.Run(async (HttpContext context) =>
            {
                //context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                
                // The HttpContext API that applications and middleware use to process
                // requests has an abstraction layer underneath it called feature
                // interfaces.
                // Each feature interface provides a granular subset of the
                // functionality exposed by HttpContext.
                // These interfaces can be added, modified, wrapped, replaced, or even
                // removed by the server or middleware as the request is processed
                // without having to re-implement the entire HttpContext.
                // They can also be used to mock functionality when testing.
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    // Additionally, you can see the advantage of using the base abstract 
                    // exception class here (NotFoundException in this case). We are not 
                    // checking for the specific class implementation but the base type. This 
                    // allows us to have multiple not found classes that inherit from the 
                    // NotFoundException class and this middleware will know that we want to 
                    // return the NotFound response to the client.
                    // With this approach, we have perfect control of all the exceptional
                    // cases in our app. We have that control due to global error handler
                    // implementation.
                    context.Response.StatusCode = contextFeature.Error switch
                    {
                        NotFoundException => StatusCodes.Status404NotFound,
                        _ => StatusCodes.Status500InternalServerError
                    };
                    
                    logger.LogError($"Something went wrong: {contextFeature.Error}");
                    
                    await context.Response.WriteAsync(new ErrorDetails
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = contextFeature.Error.Message,
                    }.ToString());
                }
            });
        });
    }
}