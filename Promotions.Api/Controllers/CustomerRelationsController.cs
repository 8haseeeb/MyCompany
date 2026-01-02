using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Promotions.Application.CustomerRelations.Commands;
using Promotions.Application.CustomerRelations.Dtos;
using Promotions.Application.CustomerRelations.Queries;

[ApiController]
[Route("api/promotions/customer-relations")]
[Authorize]
public class CustomerRelationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerRelationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCustomerRelationDto dto)
    {
        await _mediator.Send(new CreateCustomerRelationCommand(
            dto.CodHier,
            dto.CodDiv,
            dto.CodNode,
            dto.IdLevel,
            dto.DteStart,
            dto.CodParentNode,
            dto.DteEnd));

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetCustomerRelationsQuery());
        return Ok(result);
    }

    [HttpGet("{codHier}/{codDiv}/{codNode}/{idLevel}/{dteStart}")]
    public async Task<IActionResult> GetById(
    string codHier,
    string codDiv,
    string codNode,
    int idLevel,
    DateTime dteStart)
    {
        var result = await _mediator.Send(
            new GetCustomerRelationByIdQuery(
                codHier,
                codDiv,
                codNode,
                idLevel,
                dteStart));

        return Ok(result);
    }


    [HttpPut]
    public async Task<IActionResult> Update(
        [FromBody] UpdateCustomerRelationDto dto,
        [FromQuery] string codHier,
        [FromQuery] string codDiv,
        [FromQuery] string codNode,
        [FromQuery] int idLevel,
        [FromQuery] DateTime dteStart)
    {
        await _mediator.Send(new UpdateCustomerRelationCommand(
            codHier,
            codDiv,
            codNode,
            idLevel,
            dteStart,
            dto.CodParentNode,
            dto.DteEnd));

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(
        [FromQuery] string codHier,
        [FromQuery] string codDiv,
        [FromQuery] string codNode,
        [FromQuery] int idLevel,
        [FromQuery] DateTime dteStart)
    {
        await _mediator.Send(new DeleteCustomerRelationCommand(
            codHier,
            codDiv,
            codNode,
            idLevel,
            dteStart));

        return NoContent();
    }
}
