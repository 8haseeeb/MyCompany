using MediatR;
using Promotions.Domain.Participants;
using Promotions.Domain.CustomerRelations;
using Promotions.Application.Common.Interfaces;


namespace Promotions.Application.Participants.Commands
{
    public class CreateParticipantCommandHandler : IRequestHandler<CreateParticipantCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateParticipantCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(CreateParticipantCommand request, CancellationToken cancellationToken)
        {
            // Auto-create CustomerRelation if it doesn't exist
            var exists = await _unitOfWork.CustomerRelations.ExistsAsync(
                request.CodHier,
                request.CodDiv,
                request.CodNode,
                request.IdLevel,
                request.DteStart);

            if (!exists)
            {
                var newRelation = new CustomerRelation(
                    request.CodHier,
                    request.CodDiv,
                    request.CodNode,
                    request.IdLevel,
                    request.DteStart,
                    "ROOT"
                );
                await _unitOfWork.CustomerRelations.AddAsync(newRelation);
            }

            var participant = new PromoParticipants(
                request.IdAction,
                request.CodParticipant,
                request.FlgInclusion,
                request.CodHier,
                request.CodDiv,
                request.CodNode,
                request.IdLevel,
                request.DteStart
            );

            await _unitOfWork.Participants.AddAsync(participant);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
