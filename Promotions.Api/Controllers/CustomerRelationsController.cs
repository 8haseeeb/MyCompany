using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Promotions.Application.CustomerRelations.Commands;
using Promotions.Application.CustomerRelations.Dtos;
using Promotions.Application.CustomerRelations.Queries;

[ApiController]
[Route("api/v1/promotions/customer-relations")]
[ApiVersion("1.0")]
[Authorize]
public class CustomerRelationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CustomerRelationsController> _logger;

    public CustomerRelationsController(IMediator mediator, ILogger<CustomerRelationsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCustomerRelationDto dto)
    {
        try
        {
            _logger.LogInformation("[CustomerRelation] Creating relation. CodHier: {CodHier}, CodNode: {CodNode}", dto.CodHier, dto.CodNode);

            await _mediator.Send(new CreateCustomerRelationCommand(
                dto.CodHier,
                dto.CodDiv,
                dto.CodNode,
                dto.IdLevel,
                dto.DteStart,
                dto.CodParentNode,
                dto.DteEnd));

            _logger.LogInformation("[CustomerRelation] Created successfully. CodHier: {CodHier}, CodNode: {CodNode}", dto.CodHier, dto.CodNode);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CustomerRelation] Create failed. CodHier: {CodHier}, CodNode: {CodNode}, Error: {Error}", dto.CodHier, dto.CodNode, ex.Message);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? idAction)
    {
        var result = await _mediator.Send(new GetCustomerRelationsQuery(idAction));
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
        try
        {
            _logger.LogInformation("[CustomerRelation] Updating. CodHier: {CodHier}, CodNode: {CodNode}", codHier, codNode);

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CustomerRelation] Update failed. CodHier: {CodHier}, CodNode: {CodNode}, Error: {Error}", codHier, codNode, ex.Message);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(
        [FromQuery] string codHier,
        [FromQuery] string codDiv,
        [FromQuery] string codNode,
        [FromQuery] int idLevel,
        [FromQuery] DateTime dteStart)
    {
        try
        {
            _logger.LogInformation("[CustomerRelation] Deleting. CodHier: {CodHier}, CodNode: {CodNode}", codHier, codNode);

            await _mediator.Send(new DeleteCustomerRelationCommand(
                codHier,
                codDiv,
                codNode,
                idLevel,
                dteStart));

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CustomerRelation] Delete failed. CodHier: {CodHier}, CodNode: {CodNode}, Error: {Error}", codHier, codNode, ex.Message);
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
