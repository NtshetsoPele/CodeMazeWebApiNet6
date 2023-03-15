using Contracts;
using LoggerService;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Repository;
using Service;
using Service.Contracts;

namespace CompanyEmployees.Extensions;

public static class ServiceExtensions
{
    // CORS (Cross-Origin Resource Sharing) is a mechanism
    // to give or restrict access rights to applications
    // from different domains.
    // If we want to send requests from a different domain
    // to our application, configuring CORS is mandatory.
    
    // We are using basic CORS policy settings because allowing any origin, 
    // method, and header is okay for now. But we should be more 
    // restrictive with those settings in the production environment. More 
    // precisely, as restrictive as possible.
    
    // Instead of the AllowAnyOrigin() method which allows requests from any 
    // source, we can use the WithOrigins("https://example.com") which will 
    // allow requests only from that concrete source. Also, instead of
    // AllowAnyMethod() that allows all HTTP methods, we can use
    // WithMethods("POST", "GET") that will allow only specific HTTP methods.
    // Furthermore, you can make the same changes for the AllowAnyHeader()
    // method by using, for example, the WithHeaders("accept", "content-type")
    // method to allow only specific headers.
    public static void ConfigureCors(this IServiceCollection services) => 
        services.AddCors((CorsOptions options) => 
        {
            options.AddPolicy(name: "CorsPolicy", (CorsPolicyBuilder builder) => 
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
    
    // ASP.NET Core applications are by default self-hosted, and if we want to 
    // host our application on IIS, we need to configure an IIS integration which 
    // will eventually help us with the deployment to IIS.
    public static void ConfigureIisIntegration(this IServiceCollection services) =>
        services.Configure<IISOptions>((IISOptions options) =>
        {
        });
    
    public static void ConfigureLoggerService(this IServiceCollection services) =>
        services.AddSingleton<ILoggerManager, LoggerManager>();
    
    // But, as you could see, we have the RepositoryManager service registration,
    // which happens at runtime, and during that registration, we must have RepositoryContext
    // registered as well in the runtime, so we could inject it into other services (like
    // RepositoryManager service). This might be a bit confusing, so let’s see what that
    // means for us.
    
    // We are not specifying the MigrationAssembly inside the UseSqlServer method. We don’t
    // need it in this case.
    
    // One additional thing. From .NET 6 RC2, there is a shortcut method AddSqlServer, which
    // can be used like this:
    // public static void ConfigureSqlContext(this IServiceCollection services, 
    //     IConfiguration configuration) =>
    //     services.AddSqlServer<RepositoryContext>((configuration.GetConnectionString("sq
    // lConnection")));
    // This method replaces both AddDbContext and UseSqlServer methods and allows an easier
    // configuration. But it doesn’t provide all of the features the AddDbContext method
    // provides. So for more advanced options, it is recommended to use AddDbContext.
    // We will use it throughout the rest of the project.
    public static void ConfigureSqlContext(this IServiceCollection services, 
        IConfiguration configuration) =>
        services.AddDbContext<RepositoryContext>((DbContextOptionsBuilder opts) =>
            opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));
    
    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();
    
    public static void ConfigureServiceManager(this IServiceCollection services) =>
        services.AddScoped<IServiceManager, ServiceManager>();
}

