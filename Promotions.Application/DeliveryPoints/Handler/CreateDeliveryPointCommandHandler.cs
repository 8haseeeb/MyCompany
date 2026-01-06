using MediatR;
using Promotions.Application.DeliveryPoints.Interfaces;
using Promotions.Domain.DeliveryPoints;

namespace Promotions.Application.DeliveryPoints.Commands
{
    public class CreateDeliveryPointCommandHandler
        : IRequestHandler<CreateDeliveryPointCommand, Unit>
    {
        private readonly IDeliveryPointRepository _repository;

        public CreateDeliveryPointCommandHandler(
            IDeliveryPointRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(
            CreateDeliveryPointCommand request,
            CancellationToken cancellationToken)
        {
            var entity = new PromoDeliveryPoint
            {
                IdAction = request.IdAction,
                CodDeliveryPoint = request.CodDeliveryPoint,
                FlgInclusion = request.FlgInclusion,
                CodHier = request.CodHier,
                CodDiv = request.CodDiv,
                CodNode = request.CodNode,
                IdLevel = request.IdLevel,
                DteStart = request.DteStart
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
