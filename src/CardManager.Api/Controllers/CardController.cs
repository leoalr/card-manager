using CardManager.Domain.Requests;
using CardManager.Domain.Responses;
using CardManager.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace CardManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private const int DEFAULT_TOKEN_SIZE = 4;
        private const int DEFAULT_CARD_NUMBER_MAX_NUMBER_OF_DIGITS = 16;
        private const int DEFAULT_CVV_MAX_NUMBER_OF_DIGITS = 5;

        private readonly ILogger<CardController> _logger;
        private readonly ICardService _cardService;

        private readonly int _tokenSize;
        private readonly int _cardNumberMaxNumberOfDigits;
        private readonly int _cvvMaxNumberOfDigits;

        public CardController(ILogger<CardController> logger,
            IConfiguration configuration,
            ICardService cardService)
        {
            _logger = logger;
            _cardService = cardService;

            string cardTokenSize = configuration["Card:TokenSize"];
            string cardNumberMaxNumberOfDigits = configuration["Card:NumberMaxNumberOfDigits"];
            string cardCVVMaxNumberOfDigits = configuration["Card:CVVMaxNumberOfDigits"];

            _tokenSize = string.IsNullOrEmpty(cardTokenSize)
                ? DEFAULT_TOKEN_SIZE
                : Convert.ToInt32(cardTokenSize);

            _cardNumberMaxNumberOfDigits = string.IsNullOrEmpty(cardNumberMaxNumberOfDigits)
                ? DEFAULT_CARD_NUMBER_MAX_NUMBER_OF_DIGITS
                : Convert.ToInt32(cardNumberMaxNumberOfDigits);

            _cvvMaxNumberOfDigits = string.IsNullOrEmpty(cardCVVMaxNumberOfDigits)
                ? DEFAULT_CVV_MAX_NUMBER_OF_DIGITS
                : Convert.ToInt32(cardCVVMaxNumberOfDigits);
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RegisterCardResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterCard([FromBody] RegisterCardRequest request)
        {
            if (request.IsValid(_tokenSize, _cardNumberMaxNumberOfDigits, _cvvMaxNumberOfDigits) is false)
            {
                return BadRequest(new ErrorResponse(
                    "The request data is not valid. All the request body properties are required and must be greater than 0 (zero). " +
                    string.Format("The property 'cardNumber' must have between {0} and {1} digits. ", 
                        _tokenSize, _cardNumberMaxNumberOfDigits) +
                    string.Format("The property 'cvv' must have {0} digits at the maximum.", _cvvMaxNumberOfDigits)));
            }

            try
            {
                var response = await _cardService.RegisterCard(request);

                return CreatedAtAction(nameof(GetById), new { id = response.CardId }, response);
            }
            catch (Exception)
            {
                return StatusCode(500, new ErrorResponse("An unexpected error has occurred."));
            }
        }

        [HttpGet("{id:int}", Name = "GetById")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GetByIdResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var response = await _cardService.GetById(id);

                return Ok(response);
            }
            catch (ApplicationException ex)
            {
                return NotFound(new ErrorResponse(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, new ErrorResponse("An unexpected error has occurred."));
            }
        }

        [HttpPost("{id:int}/TokenValidation")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ValidateCardTokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValidateCardToken(int id, [FromBody]ValidateCardTokenRequest request)
        {
            if (request.IsValid(_tokenSize, _cvvMaxNumberOfDigits) is false)
            {
                return BadRequest(new ErrorResponse(
                    "The request data is not valid. The request body properties 'customerId', 'token' and 'cVV' are mandatory " +
                    "and their values must be greater than 0 (zero). " +
                    string.Format("The property 'token' must have {0} digits at the maximum. ",
                        _tokenSize, _tokenSize) +
                    string.Format("The property 'cvv' must have {0} digits at the maximum.", _cvvMaxNumberOfDigits)));
            }

            try
            {
                var response = await _cardService.ValidateToken(id, request);

                return Ok(response);
            }
            catch (ApplicationException ex)
            {
                return NotFound(new ErrorResponse(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, new ErrorResponse("An unexpected error has occurred."));
            }
        }
    }
}
