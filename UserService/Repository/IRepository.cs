using Microsoft.AspNetCore.Mvc;
using UserService.Models;

namespace UserService.Repository;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetAsync(Guid id);
    Task<T> CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}
