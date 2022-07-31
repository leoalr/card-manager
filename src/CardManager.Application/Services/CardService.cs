using CardManager.Adapters.EF.Contexts;
using CardManager.Domain.Entities;
using CardManager.Domain.Requests;
using CardManager.Domain.Responses;
using CardManager.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CardManager.Application.Services;

public class CardService : ICardService
{
    private const int DEFAULT_TOKEN_SIZE = 4;
    private const int DEFAULT_TOKEN_EXPIRING_TIME_IN_MINUTES = 30;

    private readonly ILogger<CardService> _logger;
    private readonly CardManagerContext _dbContext;

    private readonly int _tokenSize;
    private readonly int _tokenExpiringTimeInMinutes;

    public CardService(ILogger<CardService> logger,
        IConfiguration configuration,
        CardManagerContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;

        string tokenSize = configuration["Card:TokenSize"];
        string tokenExpiringTimeInMinutes = configuration["Card:TokenExpiringTimeInMinutes"];

        _tokenSize = string.IsNullOrEmpty(tokenSize)
            ? DEFAULT_TOKEN_SIZE
            : Convert.ToInt32(tokenSize);
        _tokenExpiringTimeInMinutes = string.IsNullOrEmpty(tokenExpiringTimeInMinutes)
            ? DEFAULT_TOKEN_EXPIRING_TIME_IN_MINUTES
            : Convert.ToInt32(tokenExpiringTimeInMinutes);
    }

    public async Task<RegisterCardResponse> RegisterCard(RegisterCardRequest request)
    {
        var card = new Card(request.CustomerId, request.CardNumber, request.CVV);

        await _dbContext.Cards.AddAsync(card);
        await _dbContext.SaveChangesAsync();

        var cardToken = card.GenerateToken(_tokenSize);

        return new RegisterCardResponse(card.Id, cardToken, card.RegistrationDate);
    }

    public async Task<GetByIdResponse> GetById(int id)
    {
        var card = await RetrieveCard(id);

        return new GetByIdResponse(card.Id, card.RegistrationDate);
    }

    public async Task<ValidateCardTokenResponse> ValidateToken(int cardId, ValidateCardTokenRequest request)
    {
        var response = new ValidateCardTokenResponse();
        response.Validated = false;

        var card = await RetrieveCard(cardId);

        if (card.HasExpired(_tokenExpiringTimeInMinutes))
        {
            return response;
        }

        if (card.IsOwnedBy(request.CustomerId) is false)
        {
            return response;
        }

        _logger.LogInformation(string.Format("Trying to validate card token for card number: {0}", card.Number));

        var cardToken = card.GenerateToken(_tokenSize, request.CVV);

        response.Validated = request.Token == cardToken;

        return response;
    }

    private async Task<Card> RetrieveCard(int id)
    {
        var card = await _dbContext.Cards.FindAsync(id);

        if (card is null)
        {
            throw new ApplicationException("A card was not found for the provided ID.");
        }

        return card;
    }
}
