using MediatR;
using Promotions.Application.Participant.Interfaces;
using Promotions.Domain.Participants;

namespace Promotions.Application.Participants.Commands
{
    public class UpdateParticipantCommandHandler : IRequestHandler<UpdateParticipantCommand, Unit>
    {
        private readonly IParticipantRepository _repository;

        public UpdateParticipantCommandHandler(IParticipantRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateParticipantCommand request, CancellationToken cancellationToken)
        {
            var participant = await _repository.GetByIdAsync(request.IdAction, request.CodParticipant);
            if (participant == null)
                throw new Exception("Participant not found");

            participant.FlgInclusion = request.FlgInclusion;

            await _repository.UpdateAsync(participant);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
