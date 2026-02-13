using MediatR;
using Promotions.Domain.Participants;
using Promotions.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace Promotions.Application.Participants.Commands
{
    public class UpdateParticipantCommandHandler : IRequestHandler<UpdateParticipantCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateParticipantCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateParticipantCommand request, CancellationToken cancellationToken)
        {
            var participant = await _unitOfWork.Participants.GetByIdAsync(request.IdAction, request.CodParticipant);
            if (participant == null)
                throw new Exception("Participant not found");

            if (request.FlgInclusion)
                participant.Include();
            else
                participant.Exclude();

            await _unitOfWork.Participants.UpdateAsync(participant);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
