using MediatR;
using Promotions.Domain.Participants;
using Promotions.Application.Participant.Interfaces;
using Promotions.Application.CustomerRelations.Interfaces;
using Promotions.Domain.CustomerRelations;

namespace Promotions.Application.Participants.Commands
{
    public class CreateParticipantCommandHandler : IRequestHandler<CreateParticipantCommand, Unit>
    {
        private readonly IParticipantRepository _repository;
        private readonly ICustomerRelationRepository _customerRelationRepository;

        public CreateParticipantCommandHandler(
            IParticipantRepository repository,
            ICustomerRelationRepository customerRelationRepository)
        {
            _repository = repository;
            _customerRelationRepository = customerRelationRepository;
        }

        public async Task<Unit> Handle(CreateParticipantCommand request, CancellationToken cancellationToken)
        {
            // Auto-create CustomerRelation if it doesn't exist
            var exists = await _customerRelationRepository.ExistsAsync(
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
                await _customerRelationRepository.AddAsync(newRelation);
                await _customerRelationRepository.SaveChangesAsync(cancellationToken);
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

            await _repository.AddAsync(participant);
            await _repository.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
