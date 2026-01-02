using MediatR;
using Promotions.Application.DeliveryPoints.Interfaces;

namespace Promotions.Application.DeliveryPoints.Commands
{
    public class DeleteDeliveryPointCommandHandler
        : IRequestHandler<DeleteDeliveryPointCommand, Unit>
    {
        private readonly IDeliveryPointRepository _repository;

        public DeleteDeliveryPointCommandHandler(
            IDeliveryPointRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(
            DeleteDeliveryPointCommand request,
            CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(
                request.IdAction,
                request.CodDeliveryPoint);

            if (entity == null)
                throw new Exception("Delivery Point not found");

            await _repository.DeleteAsync(entity);
            return Unit.Value;
        }
    }
}
