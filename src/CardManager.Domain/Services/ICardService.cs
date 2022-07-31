using CardManager.Domain.Entities;
using CardManager.Domain.Requests;
using CardManager.Domain.Responses;

namespace CardManager.Domain.Services;

public interface ICardService
{
    Task<RegisterCardResponse> RegisterCard(RegisterCardRequest request);

    Task<GetByIdResponse> GetById(int id);

    Task<ValidateCardTokenResponse> ValidateToken(int cardId, ValidateCardTokenRequest request);
}
