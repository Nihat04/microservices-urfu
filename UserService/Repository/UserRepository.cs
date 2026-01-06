using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;

namespace UserService.Repository;

public class UserRepository :  RepositoryBase<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var res = await _dbSet.Where(x => x.Email == email).FirstOrDefaultAsync();
        return res;
    }
}