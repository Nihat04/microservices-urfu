using UserService.Models;

namespace UserService.Repository;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}