using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Promotions.Application.Common.Interfaces;


namespace Promotions.Application.DeliveryPoints.Commands
{
    public class UpdateDeliveryPointCommandHandler : IRequestHandler<UpdateDeliveryPointCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDeliveryPointCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateDeliveryPointCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.DeliveryPoints.GetByIdAsync(request.IdAction, request.CodDeliveryPoint);
            
            if (entity == null)
                throw new Exception("Delivery Point not found.");

            if (request.FlgInclusion)
                entity.Include();
            else
                entity.Exclude();

            await _unitOfWork.DeliveryPoints.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
