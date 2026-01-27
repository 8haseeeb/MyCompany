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
            try
            {
                var participant = await _repository.GetByIdAsync(request.IdAction, request.CodParticipant);
                if (participant == null)
                {
                    // Using a more specific message that might help identify if it's a lookup issue
                    throw new KeyNotFoundException($"Participant with IdAction {request.IdAction} and CodParticipant '{request.CodParticipant}' not found.");
                }

                await _repository.DeleteAsync(participant);
                await _repository.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
            catch (Exception ex)
            {
                // Rethrowing to let global exception handler catch it, but could add logging here if needed.
                // For now, let's make sure the exception contains enough info.
                throw new Exception($"Error deleting participant: {ex.Message}", ex);
            }
        }
    }
}
