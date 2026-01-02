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

            entity.FlgInclusion = request.FlgInclusion;

            await _repository.UpdateAsync(entity);
            return Unit.Value;
        }
    }
}
