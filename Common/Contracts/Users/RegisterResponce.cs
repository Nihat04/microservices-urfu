namespace Common.Contracts.Users;

public record RegisterResponce(
    Guid Id,
    string Name,
    string Email
    );