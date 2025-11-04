namespace Application.Exceptions;

/// <summary>
/// Classe base para exceções de negócio/domínio.
/// Exceções de negócio representam violações de regras de negócio e geralmente resultam em códigos 4xx.
/// </summary>
public abstract class BusinessException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }

    protected BusinessException(
        string message, 
        int statusCode, 
        string errorCode) 
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    protected BusinessException(
        string message, 
        int statusCode, 
        string errorCode,
        Exception innerException) 
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}
