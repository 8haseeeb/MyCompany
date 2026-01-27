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
