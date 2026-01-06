using UserService.Data;
using UserService.Models;

namespace UserService.Repository;

public class UserRepository : RepositoryBase<User>
{
    public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }
}