using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Promotions.Application.Products.Commands;
using Promotions.Application.Products.Dtos;
using Promotions.Application.Products.Queries;

namespace Promotions.Api.Controllers
{
    [ApiController]
    [Route("api/promotions/products")]
    [Authorize]
    [Tags("Promotion Products")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateProductDto dto)
        {
            await _mediator.Send(new CreateProductCommand(
                dto.IdAction,
                dto.CodProduct,
                dto.LevProduct,
                dto.CodDisplay,
                dto.CodDiv,
                dto.QtyEstimated,
                dto.PerceDiscount1,
                dto.PerceDiscount2,
                dto.NumMeasure,
                dto.CodMeasure));

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllProductsQuery());
            return Ok(result);
        }

        [HttpGet("action/{idAction}")]
        public async Task<IActionResult> GetByAction(int idAction)
        {
            var result = await _mediator.Send(
                new GetProductsByActionQuery(idAction));

            return Ok(result);
        }

        [HttpGet("{idAction}/{codProduct}/{levProduct}/{codDisplay}")]
        public async Task<IActionResult> GetById(
            int idAction,
            string codProduct,
            int levProduct,
            string codDisplay)
        {
            var result = await _mediator.Send(
                new GetProductByIdQuery(
                    idAction,
                    codProduct,
                    levProduct,
                    codDisplay));

            return Ok(result);
        }

       
        [HttpPut]
        public async Task<IActionResult> Update(
            [FromBody] UpdateProductDto dto,
            [FromQuery] int idAction,
            [FromQuery] string codProduct,
            [FromQuery] int levProduct,
            [FromQuery] string codDisplay)
        {
            await _mediator.Send(new UpdateProductCommand(
                idAction,
                codProduct,
                levProduct,
                codDisplay,
                dto.CodDiv,
                dto.QtyEstimated,
                dto.PerceDiscount1,
                dto.PerceDiscount2,
                dto.NumMeasure,
                dto.CodMeasure));

            return NoContent();
        }

        
        [HttpDelete]
        public async Task<IActionResult> Delete(
            [FromQuery] int idAction,
            [FromQuery] string codProduct,
            [FromQuery] int levProduct,
            [FromQuery] string codDisplay)
        {
            await _mediator.Send(new DeleteProductCommand(
                idAction,
                codProduct,
                levProduct,
                codDisplay));

            return NoContent();
        }
    }
}
