using Microsoft.AspNetCore.Mvc;
using UserService.Models;

namespace UserService.Repository;

public interface IRepository<T> where T : BaseEntity
{
    public Task<ActionResult<T>> Get(Guid id);
    public Task<ActionResult<T>> Create(T entity);
    public Task<ActionResult> Update(Guid id, T entity);
    public Task<ActionResult> Delete(Guid id);
}