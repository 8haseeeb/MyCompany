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
    [Route("api/v1/promotions/actions")]
    [ApiVersion("1.0")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAtomic(
            [FromBody] AtomicCreatePromoActionDto dto)
        {
            await _mediator.Send(new CreateAtomicPromoActionCommand(dto));
            return Ok();
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(
            [FromQuery] int idAction)
        {
            await _mediator.Send(
                new DeletePromoActionCommand(idAction));

            return NoContent();
        }
    }
}
