using System.Linq.Expressions;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public abstract class RepositoryBase<T> : IRepositoryBase<T> where T: class
{
    private readonly RepositoryContext _repositoryContext;

    protected RepositoryBase(RepositoryContext repositoryContext)
        => _repositoryContext = repositoryContext;
    
    public IQueryable<T> FindAll(bool trackChanges)
    {
        return trackChanges ?
            _repositoryContext.Set<T>() :
            _repositoryContext.Set<T>().AsNoTracking();
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, 
        bool trackChanges)
    {
        return trackChanges ? 
            _repositoryContext.Set<T>().Where(expression) :
            _repositoryContext.Set<T>().Where(expression).AsNoTracking();
    }

    public void Create(T entity) =>
        _repositoryContext.Set<T>().Add(entity);

    public void Update(T entity) =>
        _repositoryContext.Set<T>().Update(entity);

    public void Delete(T entity) =>
        _repositoryContext.Set<T>().Remove(entity);
}

// We are going to use it to improve our read-only query performance.
// When it’s set to false, we attach the AsNoTracking method to our
// query to inform EF Core that it doesn’t need to track changes for
// the required entities. This greatly improves the speed of a query.