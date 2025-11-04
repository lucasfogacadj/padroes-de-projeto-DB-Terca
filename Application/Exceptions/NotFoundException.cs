namespace Application.Exceptions;

/// <summary>
/// Exceção lançada quando um recurso não é encontrado.
/// Resulta em HTTP 404 Not Found.
/// </summary>
public class NotFoundException : BusinessException
{
    public NotFoundException(string message) 
        : base(message, StatusCodes.Status404NotFound, "NOT_FOUND")
    {
    }

    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} com ID '{key}' não foi encontrado.", 
               StatusCodes.Status404NotFound, 
               "NOT_FOUND")
    {
    }
}
