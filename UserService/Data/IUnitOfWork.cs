using Microsoft.EntityFrameworkCore;

namespace UserService.Data;

public interface IUnitOfWork
{
    DbContext Context { get; }
    public Task SaveChangesAsync();   
}