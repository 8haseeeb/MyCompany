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
            try
            {
                var entity = await _repository.GetByIdAsync(
                    request.IdAction,
                    request.CodDeliveryPoint);

                if (entity == null)
                {
                    throw new KeyNotFoundException($"Delivery Point with IdAction {request.IdAction} and CodDeliveryPoint '{request.CodDeliveryPoint}' not found.");
                }

                await _repository.DeleteAsync(entity);
                await _repository.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting delivery point: {ex.Message}", ex);
            }
        }
    }
}
