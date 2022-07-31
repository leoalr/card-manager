namespace CardManager.Domain.Responses;

public class RegisterCardResponse
{
    public int CardId { get; private set; }

    public long Token { get; private set; }

    public DateTime RegistrationDate { get; private set; }

    public RegisterCardResponse(int cardId, long token, DateTime registrationDate)
    {
        CardId = cardId;
        Token = token;
        RegistrationDate = registrationDate;
    }
}
