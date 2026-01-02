using MediatR;
using Microsoft.AspNetCore.Mvc;
using Promotions.Application.Participant.Queries;
using Promotions.Application.Participants.Commands;
using Promotions.Application.Participants.Dtos;

[ApiController]
[Route("api/actions/{idAction}/participants")]
[Tags("Participants")]
public class ParticipantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ParticipantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateParticipant(CreateParticipantCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [HttpPut("{codParticipant}")]
    public async Task<IActionResult> UpdateParticipant(int idAction, string codParticipant, [FromBody] UpdateParticipantDto request)
    {
        await _mediator.Send(new UpdateParticipantCommand(idAction, codParticipant, request.FlgInclusion));
        return Ok();
    }

    [HttpDelete("{codParticipant}")]
    public async Task<IActionResult> DeleteParticipant(int idAction, string codParticipant)
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

    [HttpGet("/api/participants/all")]
    public async Task<IActionResult> GetAllParticipants()
    {
        var result = await _mediator.Send(new GetAllParticipantsQuery());
        return Ok(result);
    }
}
