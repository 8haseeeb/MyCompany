using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Promotions.Application.Common.Interfaces;


namespace Promotions.Application.DeliveryPoints.Commands
{
    public class DeleteDeliveryPointCommandHandler
        : IRequestHandler<DeleteDeliveryPointCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDeliveryPointCommandHandler(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            DeleteDeliveryPointCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _unitOfWork.DeliveryPoints.GetByIdAsync(
                    request.IdAction,
                    request.CodDeliveryPoint);

                if (entity == null)
                {
                    throw new KeyNotFoundException($"Delivery Point with IdAction {request.IdAction} and CodDeliveryPoint '{request.CodDeliveryPoint}' not found.");
                }

                await _unitOfWork.DeliveryPoints.DeleteAsync(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting delivery point: {ex.Message}", ex);
            }
        }
    }
}
