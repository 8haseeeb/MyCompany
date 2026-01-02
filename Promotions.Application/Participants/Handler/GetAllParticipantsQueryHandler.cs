using MediatR;
using Promotions.Application.Participants.Dtos;
using Promotions.Application.Participant.Interfaces;
using Promotions.Domain.Participants;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Promotions.Application.Participant.Queries;

namespace Promotions.Application.Participant.Handler

{
    public class GetAllParticipantsQueryHandler : IRequestHandler<GetAllParticipantsQuery, List<ParticipantDto>>
    {
        private readonly IParticipantRepository _repository;

        public GetAllParticipantsQueryHandler(IParticipantRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ParticipantDto>> Handle(GetAllParticipantsQuery request, CancellationToken cancellationToken)
        {
            var participants = await _repository.GetAllAsync(); 
            return participants.Select(p => new ParticipantDto
            {
                IdAction = p.IdAction,
                CodParticipant = p.CodParticipant,
                FlgInclusion = p.FlgInclusion
            }).ToList();
        }
    }
}
