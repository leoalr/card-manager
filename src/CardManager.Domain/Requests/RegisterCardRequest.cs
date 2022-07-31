namespace CardManager.Domain.Requests;

public class RegisterCardRequest
{
    public int CustomerId { get; set; }

    public long CardNumber { get; set; }

    public int CVV { get; set; }

    public bool IsValid(int tokenSize, int cardNumberMaxNumberOfDigits, int cVVMaxNumberOfDigits)
    {
        if (CustomerId <= 0)
        {
            return false;
        }

        if (CardNumber <= 0)
        {
            return false;
        }

        string cardNumberString = CardNumber.ToString();

        if (cardNumberString.Length < tokenSize)
        {
            return false;
        }

        if (cardNumberString.Length > cardNumberMaxNumberOfDigits)
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
