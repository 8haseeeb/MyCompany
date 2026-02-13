using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Promotions.Application.Common.Interfaces;


namespace Promotions.Application.Participants.Commands
{
    public class DeleteParticipantCommandHandler : IRequestHandler<DeleteParticipantCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteParticipantCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteParticipantCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var participant = await _unitOfWork.Participants.GetByIdAsync(request.IdAction, request.CodParticipant);
                if (participant == null)
                {
                    // Using a more specific message that might help identify if it's a lookup issue
                    throw new KeyNotFoundException($"Participant with IdAction {request.IdAction} and CodParticipant '{request.CodParticipant}' not found.");
                }

                await _unitOfWork.Participants.DeleteAsync(participant);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

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
