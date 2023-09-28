using System.Net;

namespace BuberDinner.Application.Common.Errors;

public class IServiceException : Exception
{
    public IServiceException(HttpStatusCode statusCode, string errorMessage) : base(errorMessage)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }
    public string ErrorMessage => Message;
}