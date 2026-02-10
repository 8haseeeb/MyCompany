using MediatR;
using Promotions.Application.DeliveryPoints.Interfaces;
using Promotions.Domain.DeliveryPoints;
using Promotions.Application.CustomerRelations.Interfaces;
using Promotions.Domain.CustomerRelations;

namespace Promotions.Application.DeliveryPoints.Commands
{
    public class CreateDeliveryPointCommandHandler
        : IRequestHandler<CreateDeliveryPointCommand, Unit>
    {
        private readonly IDeliveryPointRepository _repository;
        private readonly ICustomerRelationRepository _customerRelationRepository;

        public CreateDeliveryPointCommandHandler(
            IDeliveryPointRepository repository,
            ICustomerRelationRepository customerRelationRepository)
        {
            _repository = repository;
            _customerRelationRepository = customerRelationRepository;
        }

        public async Task<Unit> Handle(
            CreateDeliveryPointCommand request,
            CancellationToken cancellationToken)
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

            var entity = new PromoDeliveryPoint(
                request.IdAction,
                request.CodDeliveryPoint,
                request.FlgInclusion,
                request.CodHier,
                request.CodDiv,
                request.CodNode,
                request.IdLevel,
                request.DteStart
            );

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
