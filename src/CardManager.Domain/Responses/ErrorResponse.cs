namespace CardManager.Domain.Responses;

public class ErrorResponse
{
    public string ErrorMessage { get; private set; }

    public ErrorResponse(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}
