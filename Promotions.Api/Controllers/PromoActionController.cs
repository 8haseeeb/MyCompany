using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Promotions.Application.PromoActions.Commands;
using Promotions.Application.PromoActions.Dtos;
using Promotions.Application.PromoActions.Queries;
using Promotions.Application.PromotionDetails.Queries;



namespace Promotions.Api.Controllers
{
    [ApiController]
    [Route("api/promotions/actions")]
    [Authorize]
    [Tags("Promotion Actions")]
    public class PromoActionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PromoActionController> _logger;

        public PromoActionController(IMediator mediator, ILogger<PromoActionController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreatePromoActionDto dto)
        {
            await _mediator.Send(new CreatePromoActionCommand(
                dto.IdAction,
                dto.Name,
                dto.CodDiv,
                dto.DteStartSellIn,
                dto.DteEndSellIn,
                dto.DteStartSellOut,
                dto.DteEndSellOut,
                dto.DocumentKey,
                dto.DteToShost,
                dto.LevParticipants));



            return Ok();
        }

        [HttpPost("atomic")]
        public async Task<IActionResult> CreateAtomic(
            [FromBody] AtomicCreatePromoActionDto dto)
        {
            try
            {
                await _mediator.Send(new CreateAtomicPromoActionCommand(dto));
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CreateAtomic] Controller caught exception. Error: {ErrorMessage}", ex.Message);

                var messages = new List<string> { ex.Message };
                var inner = ex.InnerException;
                while (inner != null)
                {
                    messages.Add(inner.Message);
                    inner = inner.InnerException;
                }

                return StatusCode(500, new
                {
                    error = "An error occurred while creating the atomic promotion.",
                    details = messages
                });
            }
        }


        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(
                new GetAllPromoActionsQuery());

            return Ok(result);
        }

      
        [HttpGet("{idAction}")]
        public async Task<IActionResult> GetById(int idAction)
        {
            var result = await _mediator.Send(
                new GetPromoActionByIdQuery(idAction));

            return Ok(result);
        }

        [HttpGet("{idAction}/complete")]
        public async Task<IActionResult> GetComplete(int idAction)
        {
            var result = await _mediator.Send(
                new GetCompletePromotionQuery(idAction));

            if (result.PromoAction == null)
            {
                return NotFound(new { message = $"Promotion with ID {idAction} not found." });
            }

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(
            [FromBody] UpdatePromoActionDto dto,
            [FromQuery] int idAction)
        {
            await _mediator.Send(new UpdatePromoActionCommand(
                idAction,
                dto.Name,
                dto.DteStartSellIn,
                dto.DteEndSellIn,
                dto.DteStartSellOut,
                dto.DteEndSellOut,
                dto.DocumentKey,
                dto.DteToShost,
                dto.LevParticipants));

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(
            [FromQuery] int idAction)
        {
            await _mediator.Send(
                new DeletePromoActionCommand(idAction));

            return NoContent();
        }
    }
}
