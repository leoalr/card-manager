namespace CardManager.Domain.Responses;

public class GetByIdResponse
{
    public int Id { get; private set; }

    public DateTime RegistrationDate { get; private set; }

    public GetByIdResponse(int id, DateTime registrationDate)
    {
        Id = id;
        RegistrationDate = registrationDate;
    }

}
