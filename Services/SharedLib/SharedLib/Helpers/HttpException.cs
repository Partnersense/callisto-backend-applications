using System.Net;

namespace SharedLib.Helpers;

/// <summary>
/// Exception used in HttpClients
/// </summary>
public class HttpException : Exception
{
    public HttpStatusCode? StatusCode { get; }

    public HttpException(HttpStatusCode statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }
}

