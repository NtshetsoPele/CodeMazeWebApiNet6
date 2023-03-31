using Contracts;

namespace Repository;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;
    private readonly Lazy<ICompanyRepository> _companyRepository;
    private readonly Lazy<IEmployeeRepository> _employeeRepository;
    
    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext ?? 
                             throw new ArgumentNullException(nameof(repositoryContext));
        _companyRepository = new Lazy<ICompanyRepository>(() => 
            new CompanyRepository(repositoryContext));
        _employeeRepository = new Lazy<IEmployeeRepository>(() => 
            new EmployeeRepository(repositoryContext));
    }
    public ICompanyRepository Company => _companyRepository.Value;
    public IEmployeeRepository Employee => _employeeRepository.Value;
    // public void Save() => _repositoryContext.SaveChanges();
    // Using the await keyword is not mandatory.
    // If we don’t, though, our method will execute synchronously.
    public async Task SaveAsync() => await _repositoryContext.SaveChangesAsync();
}

// Leveraging the power of the Lazy class to ensure the lazy initialization
// of our repositories. This means that our repository instances are only
// going to be created when we access them for the first time, and not before
// that.