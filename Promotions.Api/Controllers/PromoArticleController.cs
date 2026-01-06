using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Promotions.Application.PromoArticles.Commands;
using Promotions.Application.PromoArticles.Dtos;
using Promotions.Application.PromoArticles.Queries;

namespace Promotions.Api.Controllers
{
    [ApiController]
    [Route("api/promotions/promo-articles")]
    [Authorize]
    [Tags("Promotion Articles")]
    public class PromoArticleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PromoArticleController(IMediator mediator)
        {
            _mediator = mediator;
        }

       
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreatePromoArticleDto dto)
        {
            await _mediator.Send(new CreatePromoArticleCommand(
                dto.IdAction,
                dto.CodProduct,
                dto.LevProduct,
                dto.CodDisplay,
                dto.CodDiv,
                dto.CodNode,
                dto.CodNode1,
                dto.CodNode2,
                dto.CodNodeN));

            return Ok();
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(
                new GetAllPromoArticlesQuery());

            return Ok(result);
        }

        
        [HttpGet("find")]
        public async Task<IActionResult> GetById(
            [FromQuery] string codDiv,
            [FromQuery] string codNode)
        {
            var result = await _mediator.Send(
                new GetPromoArticleByIdQuery(
                    codDiv,
                    codNode));

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(
            [FromBody] UpdatePromoArticleDto dto,
            [FromQuery] string codDiv,
            [FromQuery] string codNode)
        {
            await _mediator.Send(new UpdatePromoArticleCommand(
                codDiv,
                codNode,
                dto.CodNode1,
                dto.CodNode2,
                dto.CodNodeN));

            return NoContent();
        }

       
        [HttpDelete]
        public async Task<IActionResult> Delete(
            [FromQuery] string codDiv,
            [FromQuery] string codNode)
        {
            await _mediator.Send(
                new DeletePromoArticleCommand(
                    codDiv,
                    codNode));

            return NoContent();
        }
    }
}
