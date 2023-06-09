using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Repository;

namespace CompanyEmployees.ContextFactory;

// Since our RepositoryContext class is in a Repository project and not
// in the main one, this class will help our application create a derived
// DbContext instance during the design time which will help us with our migrations

// We are using the IDesignTimeDbContextFactory<out TContext> interface that allows
// design-time services to discover implementations of this interface
public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
{
    public RepositoryContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json")
            .Build();
        
        var builder = new DbContextOptionsBuilder<RepositoryContext>()
            .UseSqlServer(configuration.GetConnectionString("sqlConnection"),
                (SqlServerDbContextOptionsBuilder b) 
                    => b.MigrationsAssembly("CompanyEmployees"));
        
        return new RepositoryContext(builder.Options);
    } 
}

// With the RepositoryContextFactory class, which implements the
// IDesignTimeDbContextFactory interface, we have registered our RepositoryContext
// class at design time. This helps us find the RepositoryContext class in another
// project while executing migrations.