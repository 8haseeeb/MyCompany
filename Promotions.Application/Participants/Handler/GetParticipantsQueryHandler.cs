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
            FlgInclusion = p.FlgInclusion,
            CodHier = p.CodHier ?? p.Relation?.CodHier ?? string.Empty,
            CodDiv = p.CodDiv ?? p.Relation?.CodDiv ?? string.Empty,
            CodNode = p.CodNode ?? p.Relation?.CodNode ?? string.Empty,
            IdLevel = p.IdLevel != 0 ? p.IdLevel : (p.Relation?.IdLevel ?? 0),
            DteStart = p.DteStart != default ? p.DteStart : (p.Relation?.DteStart ?? default)
        }).ToList();
    }
}
