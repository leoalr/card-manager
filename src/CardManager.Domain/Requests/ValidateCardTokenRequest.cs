namespace CardManager.Domain.Requests;

public class ValidateCardTokenRequest
{
    public int CustomerId { get; set; }

    public long Token { get; set; }

    public int CVV { get; set; }

    public bool IsValid(int tokenSize, int cVVMaxNumberOfDigits)
    {
        if (CustomerId <= 0)
        {
            return false;
        }

        if (Token <= 0)
        {
            return false;
        }

        if (Token.ToString().Length > tokenSize)
        {
            return false;
        }


        if (CVV <= 0)
        {
            return false;
        }

        if (CVV.ToString().Length > cVVMaxNumberOfDigits)
        {
            return false;
        }

        return true;
    }
}
