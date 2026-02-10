using MediatR;
using Promotions.Application.DeliveryPoints.Interfaces;

namespace Promotions.Application.DeliveryPoints.Commands
{
    public class UpdateDeliveryPointCommandHandler : IRequestHandler<UpdateDeliveryPointCommand, Unit>
    {
        private readonly IDeliveryPointRepository _repository;

        public UpdateDeliveryPointCommandHandler(IDeliveryPointRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateDeliveryPointCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.IdAction, request.CodDeliveryPoint);
            
            if (entity == null)
                throw new Exception("Delivery Point not found.");

            if (request.FlgInclusion)
                entity.Include();
            else
                entity.Exclude();

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
