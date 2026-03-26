using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Promotions.Application.Participant.Queries;
using Promotions.Application.Participants.Commands;
using Promotions.Application.Participants.Dtos;

[ApiController]
[Route("api/v1/actions/{idAction}/participants")]
[ApiVersion("1.0")]
[Tags("Participants")]
[Authorize]
public class ParticipantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ParticipantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateParticipant([FromRoute] int idAction, [FromBody] CreateParticipantDto dto)
    {
        var command = new CreateParticipantCommand(
            idAction,
            dto.CodParticipant,
            dto.FlgInclusion,
            dto.CodHier ?? string.Empty,
            dto.CodDiv ?? string.Empty,
            dto.CodNode ?? string.Empty,
            dto.IdLevel ?? 0,
            dto.DteStart ?? DateTime.MinValue);

        await _mediator.Send(command);
        return Ok();
    }

    [HttpPut("{codParticipant}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateParticipant([FromRoute] int idAction, [FromRoute] string codParticipant, [FromBody] UpdateParticipantDto request)
    {
        await _mediator.Send(new UpdateParticipantCommand(idAction, codParticipant, request.FlgInclusion));
        return Ok();
    }

    [HttpDelete("{codParticipant}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteParticipant([FromRoute] int idAction, [FromRoute] string codParticipant)
    {
        await _mediator.Send(new DeleteParticipantCommand(idAction, codParticipant));
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetParticipants(int idAction)
    {
        var result = await _mediator.Send(new GetParticipantsByActionQuery(idAction));
        return Ok(result);
    }

    /// <summary>Unversioned path (direct Promotions.Api). Gateway rewrites clients using /api/participants/all to /api/v1/participants/all.</summary>
    [HttpGet("/api/participants/all")]
    [HttpGet("/api/v1/participants/all")]
    public async Task<IActionResult> GetAllParticipants()
    {
        var result = await _mediator.Send(new GetAllParticipantsQuery());
        return Ok(result);
    }
}
