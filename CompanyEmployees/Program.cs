using CompanyEmployees.Extensions;
using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using NLog;

LogManager.LoadConfiguration(Path.Combine(
    path1: Directory.GetCurrentDirectory(),
    path2: "./nlog.config"));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// (Where we should start with service registration)
// (A service is a reusable code that adds some
// functionality to our application)

builder.Services.ConfigureCors();
builder.Services.ConfigureIisIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();

// Enable custom responses from the actions.
// Suppresses a default model state validation that is 
// implemented due to the existence of the [ApiController]
// attribute in all API controllers.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Registers only the controllers in IServiceCollection
// and not Views or Pages because they are not required
// in the Web API project.
// Without this code, our API wouldn’t work, and wouldn’t know where to
// route incoming requests. But now, our app will find all of the controllers
// inside of the Presentation project and configure them with the framework.
// They are going to be treated the same as if they were defined conventionally.
builder.Services.AddControllers(options =>
    {
        options.RespectBrowserAcceptHeader = true;
        options.ReturnHttpNotAcceptable = true;
    })
    .AddXmlDataContractSerializerFormatters()
    .AddCustomCsvFormatter()
    .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly);

builder.Services.AddAutoMapper(typeof(Program));

// With the Build method, we are creating the app variable
// of the type WebApplication. This class (WebApplication)
// is very important since it implements multiple interfaces
// like IHost that we can use to start and stop the host,
// IApplicationBuilder that we use to build the middleware
// pipeline, and IEndpointRouteBuilder used to add endpoints
// in our app.
var app = builder.Build();

///////////////////////////////////////////////////////
///////////////////////////////////////////////////////

// Configure the HTTP request pipeline.

// It is important to know that we have to extract the
// ILoggerManager service after the var app = builder.Build()
// code line because the Build method builds the WebApplication
// and registers all the services added with IOC.
var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);

// Additionally, we remove the call to the
// UseDeveloperExceptionPage method in the development
// environment since we don’t need it now and it also
// interferes with our error handler middleware.
if (app.Environment.IsProduction())
{
    app.UseHsts();
}
/*if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}*/

// Enables using static files for the request. If we
// don’t set a path to the static files directory, it
// will use the wwwroot folder in our project by default.
app.UseStaticFiles();

// Forwards proxy headers to the current request.
// This will help us during application deployment.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseCors("CorsPolicy");

app.UseAuthorization();

// Adds the endpoints from controller actions to the
// IEndpointRouteBuilder
app.MapControllers();

app.Run();
