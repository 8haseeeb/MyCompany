using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Promotions.Application.ProductDetails.Dtos;
using Promotions.Application.ProductDetails.Queries;

namespace Promotions.Api.Controllers
{
    [ApiController]
    [Route("api/promotions/product-details")]
    [Authorize]
    [Tags("Promotion Product Details")]
    public class ProductDetailController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductDetailController(IMediator mediator)
        {
            _mediator = mediator;
        }

       
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateProductDetailDto dto)
        {
            await _mediator.Send(new CreateProductDetailCommand(
                dto.IdAction ?? 0,
                dto.CodProduct ?? string.Empty,
                dto.LevProduct ?? 0,
                dto.CodDisplay ?? string.Empty,

                dto.CodNode,
                dto.CodDiv,
                dto.FlgInclusion));

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllProductDetailsQuery());
            return Ok(result);
        }

        [HttpGet("action/{idAction}")]
        public async Task<IActionResult> GetByAction(int idAction)
        {
            var result = await _mediator.Send(
                new GetProductDetailsByActionQuery(idAction));

            return Ok(result);
        }

        
        [HttpGet("find")]
        public async Task<IActionResult> GetById(
            [FromQuery] int idAction,
            [FromQuery] string codProduct,
            [FromQuery] int levProduct,
            [FromQuery] string codDisplay,
            [FromQuery] string codNode,
            [FromQuery] string codDiv)
        {
            var result = await _mediator.Send(
                new GetProductDetailByIdQuery(
                    idAction,
                    codProduct,
                    levProduct,
                    codDisplay,
                    codNode,
                    codDiv));

            return Ok(result);
        }

      
        [HttpPut]
        public async Task<IActionResult> Update(
            [FromBody] UpdateProductDetailDto dto,
            [FromQuery] int idAction,
            [FromQuery] string codProduct,
            [FromQuery] int levProduct,
            [FromQuery] string codDisplay,
            [FromQuery] string codNode,
            [FromQuery] string codDiv)
        {
            await _mediator.Send(new UpdateProductDetailCommand(
                idAction,
                codProduct,
                levProduct,
                codDisplay,
                codNode,
                codDiv,
                dto.FlgInclusion));

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(
            [FromQuery] int idAction,
            [FromQuery] string codProduct,
            [FromQuery] int levProduct,
            [FromQuery] string codDisplay,
            [FromQuery] string codNode,
            [FromQuery] string codDiv)
        {
            await _mediator.Send(
                new DeleteProductDetailCommand(
                    idAction,
                    codProduct,
                    levProduct,
                    codDisplay,
                    codNode,
                    codDiv));

            return NoContent();
        }
    }
}
