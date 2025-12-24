using System.Net;

namespace ProductService.Application.Infrastructure;

public class Error
{
    public string Message { get; }

    public HttpStatusCode HttpStatus { get; }

    protected Error(string message, HttpStatusCode httpStatus)
    {
        Message = message;
        HttpStatus = httpStatus;
    }

    public static Error NotFound(string message = "") => new(message, HttpStatusCode.NotFound);

    public static Error Failure(string message = "") => new(message, HttpStatusCode.BadRequest);
}
