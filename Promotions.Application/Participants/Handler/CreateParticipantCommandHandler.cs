using MediatR;
using Promotions.Domain.Participants;
using Promotions.Application.Participant.Interfaces;

namespace Promotions.Application.Participants.Commands
{
    public class CreateParticipantCommandHandler : IRequestHandler<CreateParticipantCommand, Unit>
    {
        private readonly IParticipantRepository _repository;

        public CreateParticipantCommandHandler(IParticipantRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(CreateParticipantCommand request, CancellationToken cancellationToken)
        {
           

            var participant = new PromoParticipants
            {
                IdAction = request.IdAction,
                CodParticipant = request.CodParticipant,
                FlgInclusion = request.FlgInclusion
            };

            await _repository.AddAsync(participant);
            await _repository.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
