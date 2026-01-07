using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Promotions.Application.PromoActions.Commands;
using Promotions.Application.PromoActions.Dtos;
using Promotions.Application.PromoActions.Queries;

namespace Promotions.Api.Controllers
{
    [ApiController]
    [Route("api/promotions/actions")]
    [Authorize]
    [Tags("Promotion Actions")]
    public class PromoActionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PromoActionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreatePromoActionDto dto)
        {
            await _mediator.Send(new CreatePromoActionCommand(
                dto.IdAction,
                dto.Name,
                dto.CodDiv,
                dto.CodContractor,
                dto.DteStartSellIn,
                dto.DteEndSellIn,
                dto.DteStartSellOut,
                dto.DteEndSellOut,
                dto.DocumentKey,
                dto.DteToShost,
                dto.LevParticipants));

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

        [HttpPut]
        public async Task<IActionResult> Update(
            [FromBody] UpdatePromoActionDto dto,
            [FromQuery] int idAction)
        {
            await _mediator.Send(new UpdatePromoActionCommand(
                idAction,
                dto.Name,
                dto.DteEndSellIn,
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
