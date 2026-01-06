using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Promotions.Application.Measures.Commands;
using Promotions.Application.Measures.Dtos;
using Promotions.Application.Measures.Queries;

namespace Promotions.Api.Controllers
{
    [ApiController]
    [Route("api/promotions/measures")]
    [Authorize]
    [Tags("Promotion Measures")]
    public class PromotionMeasuresController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PromotionMeasuresController(IMediator mediator)
        {
            _mediator = mediator;
        }


       
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PromoMeasureFieldDto dto)
        {
            await _mediator.Send(new CreatePromoMeasureFieldCommand(
                dto.IdAction,
                dto.CodProduct,
                dto.LevProduct,
                dto.CodDisplay,
                dto.CodDiv,
                dto.CodMeasure,
                dto.FieldName,
                dto.Formula));
            return Ok();
        }        
        [HttpGet]
        public async Task<IActionResult> GetAllPromoMeasures()
        {
            var result = await _mediator.Send(new GetAllPromoMeasureFieldsQuery());
            return Ok(result);
        }
        
        [HttpGet("{codMeasure}/{codDiv}/{fieldName}")]
        public async Task<IActionResult> Get(
            string codMeasure,
            string codDiv,
            string fieldName)
        {
            var result = await _mediator.Send(
                new GetPromoMeasureFieldQuery(codMeasure, codDiv, fieldName));

            return Ok(result);
        }

       
        [HttpPut("{codMeasure}/{codDiv}/{fieldName}")]
        public async Task<IActionResult> Update(
            string codMeasure,
            string codDiv,
            string fieldName,
            [FromBody] UpdatePromoMeasureFieldDto dto)
        {
            var command = new UpdatePromoMeasureFieldCommand(
                codMeasure,
                codDiv,
                fieldName,
                dto.Formula);

            await _mediator.Send(command);
            return NoContent();
        }

        
        [HttpDelete("{codMeasure}/{codDiv}/{fieldName}")]
        public async Task<IActionResult> Delete(
            string codMeasure,
            string codDiv,
            string fieldName)
        {
            var command = new DeletePromoMeasureFieldCommand(
                codMeasure, codDiv, fieldName);

            await _mediator.Send(command);
            return NoContent();
        }
    }
}
