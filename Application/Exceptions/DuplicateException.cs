namespace Application.Exceptions;

/// <summary>
/// Exceção lançada quando há tentativa de criar um recurso que já existe.
/// Resulta em HTTP 409 Conflict.
/// </summary>
public class DuplicateException : BusinessException
{
    public DuplicateException(string message) 
        : base(message, StatusCodes.Status409Conflict, "DUPLICATE")
    {
    }

    public DuplicateException(string resourceName, string field, object value)
        : base($"{resourceName} com {field} '{value}' já existe.", 
               StatusCodes.Status409Conflict, 
               "DUPLICATE")
    {
    }
}
