namespace Common.Contracts.Users;

public record RegisterRequest
{
    public required string Email { get; set; } 
    public required string FullName { get; set; }
    public required string Password { get; set; }
}
