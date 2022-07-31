namespace CardManager.Domain.Entities;

public class Card
{
    private readonly int _id;

    private readonly int _customerId;

    private readonly long _number;

    private readonly int _cvv;

    private readonly DateTime _registrationDate;

    public Card(int customerId, long number, int cvv)
    {
        _customerId = customerId;
        _number = number;
        _cvv = cvv;
        _registrationDate = DateTime.UtcNow;
    }

    public int Id
    {
        get { return _id; }
        private set { }
    }

    public long Number
    {
        get { return _number; }
        private set { }
    }

    public DateTime RegistrationDate
    {
        get { return _registrationDate; }
        private set { }
    }

    public long GenerateToken(int tokenSize, int? cvv = null)
    {
        cvv ??= _cvv;

        string cardNumber = _number.ToString();
        char[] lastDigits = cardNumber
            .Substring(cardNumber.Length - tokenSize, tokenSize)
            .ToArray();
        int rotations = cvv.Value % tokenSize;

        while (rotations > 0)
        {
            char lastDigit = lastDigits[lastDigits.Length - 1];
            for (int s = lastDigits.Length - 1; s > 0; s--)
            {
                lastDigits[s] = lastDigits[s - 1];
            }
            lastDigits[0] = lastDigit;
            
            rotations--;
        }

        return Convert.ToInt64(string.Join("", lastDigits));
    }

    public bool HasExpired(int tokenExpiringTimeInMinutes)
    {
        return DateTime.UtcNow.Subtract(_registrationDate).Minutes > tokenExpiringTimeInMinutes;
    }

    public bool IsOwnedBy(int customerId)
    {
        return customerId == _customerId;
    }
}
