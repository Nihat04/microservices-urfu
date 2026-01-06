namespace OrderService.Application.Infrastructure;

public class Result<T>
{
    private readonly T? value;
    public T Value => value!;
    private readonly Error? error;
    public Error Error => error!;
    public bool IsError => error != null;

    protected Result(T value) => this.value = value;

    protected Result(Error error) => this.error = error;

    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(Error error) => new(error);
}
