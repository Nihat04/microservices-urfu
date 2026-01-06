namespace Common.Contracts.Users;

public record AuthResponce
{
    public required string Token { get; set; }
}