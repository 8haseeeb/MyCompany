using MediatR;
using Promotions.Application.Participant.Interfaces;
using Promotions.Domain.Participants;

namespace Promotions.Application.Participants.Commands
{
    public class DeleteParticipantCommandHandler : IRequestHandler<DeleteParticipantCommand, Unit>
    {
        private readonly IParticipantRepository _repository;

        public DeleteParticipantCommandHandler(IParticipantRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteParticipantCommand request, CancellationToken cancellationToken)
        {
            var participant = await _repository.GetByIdAsync(request.IdAction, request.CodParticipant);
            if (participant == null)
                throw new Exception("Participant not found");

            await _repository.DeleteAsync(participant);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
