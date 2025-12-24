namespace ProductService.Application.Infrastructure;

public class Result
{
    private readonly Error? error;
    public Error Error => error!;
    public bool IsError => error != null;

    protected Result() { }

    protected Result(Error error) => this.error = error;

    public static Result Success() => new();

    public static implicit operator Result(Error error) => new(error);
}
