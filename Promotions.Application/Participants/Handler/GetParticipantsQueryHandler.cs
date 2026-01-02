using MediatR;
using Promotions.Application.Participants.Dtos;
using Promotions.Domain.Participants;
using Promotions.Application.Participant.Interfaces;

public class GetParticipantsByActionQueryHandler : IRequestHandler<GetParticipantsByActionQuery, List<ParticipantDto>>
{
    private readonly IParticipantRepository _repository;

    public GetParticipantsByActionQueryHandler(IParticipantRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ParticipantDto>> Handle(GetParticipantsByActionQuery request, CancellationToken cancellationToken)
    {
        var participants = await _repository.GetByActionIdAsync(request.IdAction);
        return participants.Select(p => new ParticipantDto
        {
            IdAction = p.IdAction,
            CodParticipant = p.CodParticipant,
            FlgInclusion = p.FlgInclusion
        }).ToList();
    }
}
