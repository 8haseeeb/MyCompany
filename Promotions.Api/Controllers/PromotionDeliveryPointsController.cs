using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Promotions.Application.DeliveryPoints.Commands;
using Promotions.Application.DeliveryPoints.Dtos;
using Promotions.Application.DeliveryPoints.Queries;

namespace Promotions.Api.Controllers
{
    [ApiController]
    [Route("api/promotions/delivery-points")]
    [Authorize]
    [Tags("Promotion Delivery Points")]
    public class PromotionDeliveryPointsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PromotionDeliveryPointsController(IMediator mediator)
        {
            _mediator = mediator;
        }

       
        [HttpPost("{idAction}/{codDeliveryPoint}")]
        public async Task<IActionResult> Create(
            int idAction,
            string codDeliveryPoint,
            [FromBody] CreateDeliveryPointDto dto)
        {
            var command = new CreateDeliveryPointCommand(
                idAction,
                codDeliveryPoint,
                dto.FlgInclusion,
                dto.CodHier,
                dto.CodDiv,
                dto.CodNode,
                dto.IdLevel,
                dto.DteStart);

            await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new
            {
                idAction,
                codDeliveryPoint
            }, null);
        }

        
        [HttpGet("{idAction}/{codDeliveryPoint}")]
        public async Task<IActionResult> GetById(
            int idAction,
            string codDeliveryPoint)
        {
            var result = await _mediator.Send(
                new GetDeliveryPointByIdQuery(idAction, codDeliveryPoint));

            return Ok(result);
        }

        

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllDeliveryPointsQuery());
            return Ok(result);
        }

       
        [HttpPut("{idAction}/{codDeliveryPoint}")]
        public async Task<IActionResult> Update(
            int idAction,
            string codDeliveryPoint,
            [FromBody] UpdateDeliveryPointDto dto)
        {
            var command = new UpdateDeliveryPointCommand(
                idAction,
                codDeliveryPoint,
                dto.FlgInclusion);

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{idAction}/{codDeliveryPoint}")]
        public async Task<IActionResult> Delete(
            int idAction,
            string codDeliveryPoint)
        {
            var command = new DeleteDeliveryPointCommand(
                idAction,
                codDeliveryPoint);

            await _mediator.Send(command);
            return NoContent();
        }
    }
}
