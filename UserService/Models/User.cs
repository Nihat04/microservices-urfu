namespace UserService.Models;

public class User : BaseEntity
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string HashedPassword { get; set; }
    public DateTime CreatedAt { get; set; }
}