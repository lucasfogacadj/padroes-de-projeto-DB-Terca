namespace Application.Exceptions;

/// <summary>
/// Exceção lançada quando a validação de entrada falha.
/// Resulta em HTTP 400 Bad Request.
/// </summary>
public class ValidationException : BusinessException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message) 
        : base(message, StatusCodes.Status400BadRequest, "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> errors) 
        : base("Um ou mais erros de validação ocorreram.", 
               StatusCodes.Status400BadRequest, 
               "VALIDATION_ERROR")
    {
        Errors = errors;
    }

    public ValidationException(string field, string errorMessage)
        : base($"Erro de validação no campo '{field}': {errorMessage}", 
               StatusCodes.Status400BadRequest, 
               "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { errorMessage } }
        };
    }
}
